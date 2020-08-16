# SiraUtil
 Utility library/mod for Beat Saber modders.

# For Users
 This is a utility mod, so it doesn't actually add any content or improvements to the game on its own. It's a tool for other mods to use. If you have it installed, you might have a mod that's using this!

# For Developers
 This utility contains some useful tools that might be of use. Also, this plugin has no outside dependencies! It only depends on files that are included in the base game and Harmony.

## Zenject
 This mod provides a wrapper to use Zenject, the dependency injection system that's in Beat Saber. Currently there are 4 scenes that you can inject into. AppCore (a global installer, anything injected here can be accessed anywhere, no matter which scene you're in), Menu (anything injected here is created and accessible in the menu scene), Gameplay (anything injected here is accessible in GameplayCore AND GameCore), and GameCore (anything injected in here can be accessed in GameCore only, but you can take stuff from GameplayCore).

### Alright cool, but what does this all mean?
 The base game heavily relies on using Zenject to serve components and classes to various systems and game objects. By subscribing to the Zenject pattern as a mod, this allows you to avoid the worries finding your own components as well as finding the base game components through `Resources.FindObjectsOfTypeAll`. It's incredible useful.

### How do I know which scenes contains data that's been injected?

 Almost every scene in the game has injection data inside of it. You can actually see some of the items that are available by looking through the base game and finding things that inherit from MonoInstaller. Some examples of classes which do so are:
 * AppCoreInstaller
 * PCInit
 * MenuInstaller
 * EffectPoolsInstaller
 * GameplayCoreSceneSetup
 * GameCoreSceneSetup
  
 There's quite a bit more!

 However, some of the things the base game has available can't be seen in the installer, since they're manually setup in the unity project. There's still a way to figure out if they're injected or not. For example, in the `MainFlowCoordinator`, there's quite a couple field that are marked with the `[Inject]` attribute. One of which is the `MenuLightsManager`. Obviously if the `MainFlowCoordinator` can access this, and the `MainFlowCoordinator` is in the menu, if you create your own menu installer, you can ask for the `MenuLightsManager` as well!

### Where can I learn more about Zenject?

 Before getting into this, make sure you understand Dependency Injection as a concept. Here is a quote by John Munsch which summarizes it in a way that almost anyone can understand.

 **Dependency injection for five-year-olds**
 > When you go and get things out of the refrigerator for yourself, you can cause problems. You might leave the door open, you might get something Mommy or Daddy don't want you to have. You might even be looking for something we don't even have or which has expired.

> What you should be doing is stating a need, "I need something to drink with lunch," and then we will make sure you have something when you sit down to eat.

 Despite the quote, I suggest you do your own research into Dependency Injection as a concept and maybe play around with it in a dummy project.

 [Zenject's README](https://github.com/svermeulen/Extenject) on GitHub

 [This tutorial series](https://www.youtube.com/playlist?list=PLKERDLXpXl_jNJPY2czQcfPXW4BJaGZc_) by Infallible Code. If you've got half an hour, this is a great crash course!

### How do I use Zenject via SiraUtil?

 Very simple! In the namespace `SiraUtil.Zenject`, there is a static class called `Installer`. There are a collection of methods that follow the pattern of `RegisterSCENEInstaller<T>()` and `UnregisterSCENEInstaller<T>()`. In your own mod, you create your own class, make it inherit from `MonoInstaller` and register it into the appropriate method. You only should register and unregister once, however SiraUtil won't double register an installer (that would just cause a lot of issues!). You should register and unregister in your OnEnable and OnDisable methods in your Plugin class respectively. 

 In order to use Zenject objects, reference `Zenject.dll` and `Zenject-usage.dll` in Managed.

### Example
 **System Classes**
 ```csharp
 using System;
 using Zenject;

 public class MyModGameManager : IInitializable, IDisposable
 {
     public int NotesSpawned { get; private set; }

     private BeatmapObjectManager _beatmapObjectManager;

     // Ask for the BeatmapObjectManager polietly.
     public MyModGameManager(BeatmapObjectManager beatmapObjectManager)
     {
         _beatmapObjectManager = beatmapObjectManager;
     }

     public void Initialize()
     {
         _beatmapObjectManager.noteWasSpawnedEvent += NoteWasSpawned;
     }

     public void Dispose()
     {
         _beatmapObjectManager.noteWasSpawnedEvent -= NoteWasSpawned;
     }

     private void NoteWasSpawned(NoteController controller)
     {
         NotesSpawned++;
         // Now do whatever you want with the note that was spawned.
     }
 }
 ```

 ```csharp
 using Zenject;
 using UnityEngine;

 public class MyMonoBehaviourGameManager : MonoBehaviour
 {
     private MyModGameManager _myModGameManager;
     private IDifficultyBeatmap _currentlyPlayingLevel;

     [Inject]
     public void Construct(MyModGameManager myModGameManager, IDifficultyBeatmap currentlyPlayingLevel)
     {
         _myModGameManager = myModGameManager;
         _currentlyPlayingLevel = currentlyPlayingLevel;
     }

     public void OnDestroy()
     {
         Plugin.Log.Info($"Total Notes Spawned On {_currentlyPlayingLevel.level.songName}: {_myModGameManager.NotesSpawned}");
     }
 }
 
 ```

 **A MonoInstaller**
 ```csharp
 using Zenject;

 public class ModGameInstaller : MonoInstaller
 {
     public override void InstallBindings()
     {
         Container.BindInterfacesAndSelfTo<MyModGameManager>().AsSingle();
         Container.Bind<MyMonoBehaviourGameManager>().FromNewComponentOnRoot().AsSingle();
     }
 }
 ```

 **Plugin.cs (truncated)**
 ```csharp
 [Plugin(RuntimeOptions.DynamicInit)]
 public class Plugin
 {
     [OnEnable]
     public void OnEnable()
     {
         SiraUtil.Zenject.Installer.RegisterGameplayCoreInstaller<ModGameInstaller>();
     }

     [OnDisable]
     public void OnDisable()
     {
         SiraUtil.Zenject.Installer.UnregisterGameplayCoreInstaller<ModGameInstaller>();
     }
 }
 
 ```

### Notice
When creating bindings for UI (specifically for BSML), you'll probably be using `BeatSaberUI.CreateViewController` and `BeatSaberUI.CreateFlowCoordinator`, there's an extension method on DiContainers which I've made specifically for injecting and resolving objects that are already created. Here's an example.

```csharp
public override void InstallBindings()
{
    MyViewController myViewController = BeatSaberUI.CreateViewController<MyViewController>();
    MyFlowCoordinator myFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<MyFlowCoordinator>();

    Container.InjectSpecialInstance(myViewController);
    Container.InjectSpecialInstance(myFlowCoordinator);
}
```

## Sabers

The `SiraUtil.Sabers` namespace provides a way to create new sabers. In your Plugin OnEnable and OnDisable, you can do `ExtraSabers.Touch()` and `ExtraSabers.Untouch()` to add your assembly to conditionally bind the `SiraSaber` factory in order to generate it. In an object that's been created or injected through Zenject, you can request `SiraSaber.Factory` and call .Create() on it. This returns a SiraSaber object with various methods for you to manipulate it however you'd like. There's also quite a few extension methods in `SiraUtil.Utilities` for changing normal Saber data too.

### Saber Model Providers
One of the big points of SiraUtil is to allow easier mod compatibility. This is an attempt to allow multiple saber model mods to work together. The base game binds the saber model prefab as transient, so whenever it's asked for, whatever that prefab is will be instantiated and served. SiraSabers also ask for a model controller when they are created. Modders can register their own provider by creatin a class that inherits `MonoBehaviourSaberModelController` and setting up their saber in its `Init` method. Then, create a `SaberModelProvider` class, configure it with the type of your inherited `MonoBehaviourSaberModelController` and the priority that your registration should have (higher is more priority).

### IColorable
IColorable is an interface which you can put on your saber properties so SiraUtil knows how to change its color! This is primarily implemented for use in ChromaToggle.

### Notice
Currently, any SiraSabers do not generate VRControllers. I might consider changing this in the future, but I like the idea of a mod explicitly manipulating their saber.

## Support
Have any questions about SiraUtil, Zenject, or the Sabers namespace? Reach out to me (Auros#0001) and I will gladly assist!

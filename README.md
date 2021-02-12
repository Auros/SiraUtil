# SiraUtil
 Utility library/mod for Beat Saber modders.

# For Users
 This is mainly a utility mod, so it doesn't actually add any content or improvements to the game on its own other than [SongControl](#songcontrol) and [FPFCToggle](#fpfctoggle). It's a tool for other mods to use. If you have it installed, you might have a mod that's using this!

# For Developers
 This utility contains some useful tools that might be of use. Also, this plugin has no outside dependencies! It only depends on files that are included in the base game and Harmony.

## Zenject

 This mod provides a wrapper to use Zenject, the dependency injection system that's in Beat Saber. You can inject your stuff into any container that it uses and access any items in that container.

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

 Finding every object you can request takes some assumptions and a bit of playing around. Using the [UnityRuntimeEditor](https://github.com/Caeden117/RuntimeUnityEditor) for BSIPA can allow you to easily view every provider in each scene container(s).

### Where can I learn more about Zenject?

 Before getting into this, make sure you understand Dependency Injection as a concept. Here is a quote by John Munsch which summarizes it in a way that almost anyone can understand.

 **Dependency injection for five-year-olds**
 > When you go and get things out of the refrigerator for yourself, you can cause problems. You might leave the door open, you might get something Mommy or Daddy don't want you to have. You might even be looking for something we don't even have or which has expired.

> What you should be doing is stating a need, "I need something to drink with lunch," and then we will make sure you have something when you sit down to eat.

 Despite the quote, I suggest you do your own research into Dependency Injection as a concept and maybe play around with it in a dummy project.

 If you do understand the general idea of Zenject, I HIGHLY recommend you check out these two resources. Trying to use Zenject without knowing how it works is like a chain around your leg.

 [Zenject's README](https://github.com/svermeulen/Extenject) on GitHub

 [This tutorial series](https://www.youtube.com/playlist?list=PLKERDLXpXl_jNJPY2czQcfPXW4BJaGZc_) by Infallible Code. If you've got half an hour, this is a great crash course!

### How do I use Zenject via SiraUtil?

 In your Plugin `[Init]`, you can request the `Zenjector` class from the constructor or method just like how you'd do with the BSIPA config or logger. You can then construct your bindings there.

 ```cs
 [Init]
 public Plugin(Zenjector zenjector)
 {
     zenjector.OnApp<MyMainInstaller>().WithParameters(10); // Use Zenject's installer parameter system!
     zenjector.OnMenu<MyMenuUIInstaller>();
     zenjector.OnGame<MyGameInstaller>();

     // Specify the scene name or contract or installer!
     zenject.On("Menu").Register<MyMenuEffectsInstaller>();
 }
 ```

 SiraUtil will automatically handle enabling or disabling your bindings based on the state of your plugin. You can manually enable and disable them by saving the reference to the `Zenjector` class and calling `.Enable()` and `.Disable()` on it respectively.

 In order to use Zenject types, reference `Zenject.dll` and `Zenject-usage.dll` in Managed.

### Example
 Please note that these are JUST EXAMPLES!

 **System Classes**
 ```csharp
 using System;
 using Zenject;

 public class MyModGameManager : IInitializable, IDisposable
 {
     public int NotesSpawned { get; private set; }

     private BeatmapObjectManager _beatmapObjectManager;

     // Ask for the BeatmapObjectManager polietly. The BeatmapObjectManager is a MonoBehaviour that's already injected by the base game. As long as you know it's injected, you can ask for it!
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
 
 // This shows that you can request your own components as well!
 public class MyMonoBehaviourGameManager : MonoBehaviour
 {
     private MyModGameManager _myModGameManager;
     private IDifficultyBeatmap _currentlyPlayingLevel;

     [Inject]
     public void Construct(MyModGameManager myModGameManager, IDifficultyBeatmap currentlyPlayingLevel)
     {
         // The IDifficultyBeatmap is injected into the game scene, so you can ask for it as long as you're in GameCore.
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

 // Your own installer!
 public class MyGameInstaller : Installer
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
     [Init]
     public Plugin(Zenjector zenjector)
     {
         zenjector.OnGame<MyGameInstaller>();
     }
 }
 ```

 Here's some mods that use SiraUtil's Zenject system!

 * [SiraUtil](https://github.com/Auros/SiraUtil)
 * [DiColors](https://github.com/Auros/DiColors)
 * [Enhancements](https://github.com/Auros/Enhancements)
 * [Eris' FPS Counter](https://github.com/ErisApps/FPS-Counter)
 * [Eris' SongRequestManager](https://github.com/ErisApps/SongRequestManager)
 * [Counters Plus 2.0 by Caeden](https://github.com/Caeden117/CountersPlus/tree/rewrite)
 * [Saber Factory V2 by Toni Macaroni](https://github.com/ToniMacaroni/SaberFactoryV2)
 * [Tricksaber by Toni Macaroni](https://github.com/ToniMacaroni/TrickSaber)
 * [Custom Pillows by Toni Macaroni](https://github.com/ToniMacaroni/CustomPillows)
  
### Notice
When creating bindings for view controllers and flow coordinators, you'll have to use an extension method provided by SiraUtil.

```csharp
public override void InstallBindings()
{
    Container.BindViewController<MyViewController>();
    Container.BindFLowCoordinator<MyFlowCoordinator>();
}
```

## Sabers

The `SiraUtil.Sabers` namespace provides a way to create new sabers. You can request `SiraSaber.Factory` in a game scene container and call .Create() on it. This returns a SiraSaber object with various methods for you to manipulate it however you'd like. There's also quite a few extension methods in `SiraUtil.Utilities` for changing normal Saber data too. Here's an example.

```csharp
public class MyCrazySaberMode
{
    public MyCrazySaberMode(SiraSaber.Factory factory)
    {
        SiraSaber myNewSaber = factory.Create();
        myNewSaber.ChangeType(SaberType.SaberA);
    }
}
```

SiraUtil does the heavy lifting when it comes to creating and managing sabers. Any SiraSaber's created are automatically registered into the SiraSaberEffectManager, which handles the more visual stuff like the saber burn marks on the area and the collision particles. You can manually request the SiraSaberEffectManager in Zenject if you need to, but you most likely won't need to.

### Saber Model Providers
One of the big points of SiraUtil is to allow easier mod compatibility. This is an attempt to allow multiple saber model mods to work together. The base game binds the saber model prefab as transient, so whenever it's asked for, whatever that prefab is will be instantiated and served. SiraSabers also ask for a model controller when they are created. Modders can register their own provider by creating a class that inherits both `ISaberModelController` and `IModelProvider`, configure it, and specify the priority of the model. (highest priority in the provider list gets picked as the saber to use).

### IColorable
IColorable is an interface which you can put on your objects so SiraUtil knows how to change its color! This is primarily implemented for use in ChromaToggle. This also allows mod developers to not have to depend on mod in order to change certain colors of objects. (For example, ChromaToggle does not need to reference SaberFactory in order to change the color of SaberFactory sabers, since the controller for the saber factory sabers inherit IColorable)

### Notice
Currently, any SiraSabers do not generate VRControllers. This means that you cannot expect for all sabers to have their VRControllers active.

# FPFCToggle
SiraUtil has FPFCToggle built in! You can enable it by editing SiraUtil.json in UserData and setting FPFCToggle -> Enabled to true. By default, hitting G will toggle. You can change other settings like the FOV, speed and more.

## SongControl
SiraUtil has a system like MusicEscape built in! You can enable it by editing SiraUtil.json in UserData and setting SongControl -> Enabled to true. By default, hitting Escape will exit the game, hitting F2 will pause and unpause the song, and hitting F4 will restart the level. You can change these keys by editing SiraUtil.json and putting in your own key codes.

## Support
Have any questions about SiraUtil, Zenject, or the Sabers namespace? Reach out to me (Auros#0001) and I will gladly assist! Seriously! I will convert your entire mod if you ask me to.

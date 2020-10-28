using System;
using Zenject;
using UnityEngine;
using SiraUtil.Tools;
using SiraUtil.Interfaces;

namespace SiraUtil.Zenject
{
    internal class SiraInstaller : Installer<Config, SiraInstaller>
    {
        private readonly Config _config;

        public SiraInstaller(Config config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_config).AsSingle().NonLazy();
            Container.BindInstance(_config.FPFCToggle).AsSingle();

            if (_config.FPFCToggle.Enabled)
            {
                Container.Bind<FPFCToggle>().FromNewComponentOnNewGameObject(nameof(FPFCToggle)).AsSingle().NonLazy();
            }
            Container.Bind(typeof(IInitializable), typeof(IDisposable), typeof(WebClient)).To<WebClient>().AsSingle();
            Container.BindInterfacesAndSelfTo<Localizer>().AsSingle().NonLazy();

            // Make Zenject know this is a list
            Container.Bind<IModelProvider>().To<DummyProviderA>().AsSingle();
            Container.Bind<IModelProvider>().To<DummyProviderB>().AsSingle();

            //Container.Bind<IModelProvider>().To<TestGameNoteProvider>().AsSingle();
            //Container.Bind<IModelProvider>().To<TestGameNoteProvider2>().AsSingle();
            //Container.Bind<IModelProvider>().To<TestGameNoteProvider3>().AsSingle();
        }

        private class DummyProviderA : IModelProvider { public int Priority => -9999; public Type Type => typeof(DummyProviderB); }
        private class DummyProviderB : IModelProvider { public int Priority => -9999; public Type Type => typeof(DummyProviderA); }

        private class TestGameNoteProvider : IModelProvider
        {
            public Type Type => typeof(TestGameNoteRedecoration);

            public int Priority => 300;

            private class TestGameNoteRedecoration : IPrefabProvider<GameNoteController>
            {
                public bool Chain => true;

                public GameNoteController Modify(GameNoteController original)
                {
                    var testgo = new GameObject("Test Decoration");
                    testgo.transform.SetParent(original.transform);
                    return original;
                }
            }
        }

        private class TestGameNoteProvider2 : IModelProvider
        {
            public Type Type => typeof(TestGameNoteRedecoration2);

            public int Priority => 200;

            private class TestGameNoteRedecoration2 : IPrefabProvider<GameNoteController>
            {
                public bool Chain => true;

                public GameNoteController Modify(GameNoteController original)
                {
                    var testgo = new GameObject("Test Decoration 2");
                    testgo.transform.SetParent(original.transform);
                    return original;
                }
            }
        }

        private class TestGameNoteProvider3 : IModelProvider
        {
            public Type Type => typeof(TestGameNoteRedecoration3);

            public int Priority => 100;


            private class TestGameNoteRedecoration3 : IPrefabProvider<GameNoteController>
            {
                public bool Chain => true;

                public GameNoteController Modify(GameNoteController original)
                {
                    var testgo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    testgo.transform.localScale /= 4f;
                    testgo.transform.SetParent(original.transform);
                    return original;
                }
            }
        }
    }
}
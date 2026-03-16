using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace App.Game.Bootstrap
{
    public sealed class TitleSetup : IDisposable
    {
        private readonly Action<GameMode> _onStartGame;
        private readonly GameObject _root;
        private readonly CancellationTokenSource _cts = new();

        public TitleSetup(Action<GameMode> onStartGame)
        {
            _onStartGame = onStartGame;

            _root = new GameObject("[TitleUI]");
            var uiDoc = _root.AddComponent<UIDocument>();
            uiDoc.panelSettings = Resources.Load<PanelSettings>("DefaultPanelSettings");
            uiDoc.visualTreeAsset = Resources.Load<VisualTreeAsset>("UI/TitleScreen");

            if (uiDoc.visualTreeAsset == null)
            {
                Debug.LogError("[TitleSetup] TitleScreen.uxml not found in Resources/UI/");
                return;
            }

            BindButtonsAsync(uiDoc, _cts.Token).Forget();
        }

        private async UniTaskVoid BindButtonsAsync(UIDocument uiDoc, CancellationToken ct)
        {
            await UniTask.Yield(ct);

            if (uiDoc == null || uiDoc.rootVisualElement == null) return;

            var root = uiDoc.rootVisualElement;

            root.Q<Button>("btn-new-game")?.RegisterCallback<ClickEvent>(_ => StartGame(GameMode.Manual));
            root.Q<Button>("btn-autopilot")?.RegisterCallback<ClickEvent>(_ => StartGame(GameMode.AutoPilot));
            root.Q<Button>("btn-quit")?.RegisterCallback<ClickEvent>(_ => Quit());
        }

        private void StartGame(GameMode mode)
        {
            _onStartGame?.Invoke(mode);
        }

        private static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            if (_root != null) Object.Destroy(_root);
        }
    }
}

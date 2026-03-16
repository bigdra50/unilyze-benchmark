using UnityEngine;
using UnityEngine.UIElements;

namespace App.Game.UI
{
    public sealed class MessageLog
    {
        private const int MaxMessages = 5;

        private readonly UIDocument _document;
        private ScrollView _scrollView;
        private bool _bound;

        public MessageLog(UIDocument document)
        {
            _document = document;
        }

        private void EnsureBound()
        {
            if (_bound) return;

            var root = _document != null ? _document.rootVisualElement : null;
            if (root == null) return;

            _scrollView = root.Q<ScrollView>("message-scroll");
            _bound = _scrollView != null;
        }

        public void AddMessage(string message)
        {
            EnsureBound();
            if (!_bound) return;

            var label = new Label(message);
            label.AddToClassList("message-label");
            _scrollView.Add(label);

            while (_scrollView.childCount > MaxMessages)
            {
                _scrollView.RemoveAt(0);
            }

            _scrollView.schedule.Execute(() =>
            {
                _scrollView.scrollOffset = new Vector2(0, _scrollView.contentContainer.layout.height);
            });
        }

        public void Clear()
        {
            EnsureBound();
            if (!_bound) return;

            _scrollView.Clear();
        }
    }
}

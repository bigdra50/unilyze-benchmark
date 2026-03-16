using App.Core.Models;
using App.Game;
using App.Game.Managers;
using UnityEditor;
using UnityEngine;
using VContainer;

namespace App.Editor
{
    public class DebugInspector : EditorWindow
    {
        private Vector2 _scrollPosition;

        [MenuItem("Tools/Debug Inspector")]
        private static void Open()
        {
            GetWindow<DebugInspector>("Debug Inspector");
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to inspect game state", MessageType.Info);
                return;
            }

            var scope = FindFirstObjectByType<GameLifetimeScope>();
            if (scope == null || scope.Container == null)
            {
                EditorGUILayout.HelpBox("GameLifetimeScope not found.", MessageType.Warning);
                return;
            }

            var gameManager = scope.Container.Resolve<GameManager>();
            if (gameManager == null)
            {
                EditorGUILayout.HelpBox("GameManager not found.", MessageType.Warning);
                return;
            }

            var state = gameManager.GameState;
            if (state == null)
            {
                EditorGUILayout.HelpBox("GameState is null. Start the game first.", MessageType.Warning);
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Floor & Turn
            EditorGUILayout.LabelField("Game", EditorStyles.boldLabel);
            EditorGUILayout.IntField("Floor", state.CurrentFloor);
            EditorGUILayout.IntField("Turn", state.TurnNumber);
            EditorGUILayout.Space();

            // Player
            DrawPlayer(state.Player);
            EditorGUILayout.Space();

            // Enemies
            EditorGUILayout.LabelField($"Enemies ({state.Enemies.Count})", EditorStyles.boldLabel);
            for (int i = 0; i < state.Enemies.Count; i++)
            {
                var enemy = state.Enemies[i];
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.TextField("Name", enemy.Name);
                EditorGUILayout.IntField("HP", enemy.Stats.CurrentHP);
                EditorGUILayout.TextField("Position", enemy.Position.ToString());
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();

            Repaint();
        }

        private static void DrawPlayer(Player player)
        {
            EditorGUILayout.LabelField("Player", EditorStyles.boldLabel);
            EditorGUILayout.TextField("Name", player.Name);
            EditorGUILayout.TextField("HP", $"{player.Stats.CurrentHP} / {player.Stats.MaxHP}");
            EditorGUILayout.IntField("Level", player.Level);
            EditorGUILayout.TextField("Position", player.Position.ToString());

            // Inventory
            var inventory = player.Inventory;
            EditorGUILayout.LabelField($"Inventory ({inventory.Count}/{Core.Inventory.Inventory.MaxSize})");
            EditorGUI.indentLevel++;
            for (int i = 0; i < inventory.Items.Count; i++)
            {
                EditorGUILayout.LabelField($"[{i}] {inventory.Items[i].GetName()}");
            }
            EditorGUI.indentLevel--;
        }
    }
}

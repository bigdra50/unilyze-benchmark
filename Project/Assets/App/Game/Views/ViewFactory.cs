using App.Core.Dungeon;
using App.Core.Models;
using UnityEngine;

namespace App.Game.Views
{
    public static class ViewFactory
    {
        public static GameObject CreatePlayer(Position position)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "Player";
            var worldPos = PositionToWorld(position);
            worldPos.y = 0.75f;
            go.transform.position = worldPos;
            SetUnlitColor(go, Color.white);
            return go;
        }

        public static GameObject CreateEnemy(EnemyType enemyType, Position position)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = $"Enemy_{enemyType}";
            var worldPos = PositionToWorld(position);
            worldPos.y = 0.5f;
            go.transform.position = worldPos;

            Color color;
            switch (enemyType)
            {
                case EnemyType.Slime:
                    color = new Color(0f, 0.8f, 0f);
                    break;
                case EnemyType.Goblin:
                    color = new Color(0.8f, 0f, 0f);
                    break;
                case EnemyType.Dragon:
                    color = new Color(0.6f, 0f, 0.8f);
                    break;
                default:
                    color = Color.gray;
                    break;
            }

            SetUnlitColor(go, color);
            return go;
        }

        public static GameObject CreateItem(ItemType itemType, Position position)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = $"Item_{itemType}";
            go.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            var worldPos = PositionToWorld(position);
            worldPos.y = 0.3f;
            go.transform.position = worldPos;

            Color color;
            switch (itemType)
            {
                case ItemType.HealthPotion:
                    color = new Color(1f, 0.4f, 0.7f);
                    break;
                case ItemType.AttackBoost:
                    color = new Color(1f, 0.5f, 0f);
                    break;
                case ItemType.DefenseBoost:
                    color = new Color(0.3f, 0.7f, 1f);
                    break;
                default:
                    color = Color.white;
                    break;
            }

            SetUnlitColor(go, color);
            return go;
        }

        public static GameObject CreateFloorTile(Position position)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = "Floor";
            go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            go.transform.position = PositionToWorld(position);
            SetUnlitColor(go, new Color(0.35f, 0.35f, 0.38f));
            return go;
        }

        public static GameObject CreateWallTile(Position position)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Wall";
            go.transform.localScale = new Vector3(1f, 1.5f, 1f);
            var worldPos = PositionToWorld(position);
            worldPos.y = 0.75f;
            go.transform.position = worldPos;
            SetUnlitColor(go, new Color(0.15f, 0.15f, 0.15f));
            return go;
        }

        public static GameObject CreateStairsTile(Position position)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = "Stairs";
            go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            go.transform.position = PositionToWorld(position);
            SetUnlitColor(go, new Color(1f, 0.9f, 0.2f));
            return go;
        }

        public static Vector3 PositionToWorld(Position pos)
        {
            return new Vector3(pos.X, 0f, pos.Y);
        }

        private static void SetUnlitColor(GameObject go, Color color)
        {
            var renderer = go.GetComponent<Renderer>();
            var mat = new Material(Shader.Find("Unlit/Color"));
            mat.color = color;
            renderer.material = mat;
        }
    }
}

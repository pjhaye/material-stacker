using System;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialStacking
{
    public class MaterialStacker : MonoBehaviour
    {
        private Dictionary<Renderer, MeshMaterialStack> _materialsPerRenderer = new Dictionary<Renderer, MeshMaterialStack>();

        public int StackDepth
        {
            get
            {
                return _stackDepth;
            }
        }

        private int _stackDepth = 0;
        
        private void Awake()
        {
            InitBaseMaterials();
        }

        private void InitBaseMaterials()
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                var materialList = new List<Material>();
                materialList.AddRange(renderer.materials);
                var materialsStack = new Stack<List<Material>>();
                materialsStack.Push(materialList);
                var meshMaterialStack = new MeshMaterialStack()
                {
                    MaterialsStack = materialsStack
                };
                _materialsPerRenderer.Add(renderer, meshMaterialStack);
            }
        }

        public void StackMaterialOnAllMeshes(Material material)
        {
            foreach (var kvp in _materialsPerRenderer)
            {
                var key = kvp.Key;
                var value = kvp.Value;

                var list = new List<Material>();
                var numMaterials = key.materials.Length;
                for (var i = 0; i < numMaterials; i++)
                {
                    list.Add(material);
                }
                
                value.MaterialsStack.Push(list);
            }

            _stackDepth++;
            
            UpdateMaterials();
        }

        public void PopMaterialsOnAllMeshes()
        {
            if (StackDepth == 0)
            {
                Debug.LogError($"Cannot Pop base material stack!");
                return;
            }
            
            foreach (var kvp in _materialsPerRenderer)
            {
                var value = kvp.Value;

                value.MaterialsStack.Pop();
            }

            _stackDepth--;
            
            UpdateMaterials();
        }

        private void UpdateMaterials()
        {
            foreach (var kvp in _materialsPerRenderer)
            {
                var renderer = kvp.Key;
                var materialMeshStack = kvp.Value;

                var list = materialMeshStack.MaterialsStack.Peek();
                var numMaterials = list.Count;
                
                renderer.materials = list.ToArray();
            }
        }

        public void Clear()
        {
            while (StackDepth > 0)
            {
                PopMaterialsOnAllMeshes();
            }
        }
    }
}

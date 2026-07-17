import * as THREE from 'three';

/**
 * Recursively disposes every geometry, material and material-referenced
 * texture under the given object. Each resource is disposed at most once
 * even if shared across multiple meshes. Not a generic resource manager -
 * a single-purpose cleanup function for model teardown.
 */
export function disposeObject3D(root: THREE.Object3D): void {
  const disposedGeometries = new Set<THREE.BufferGeometry>();
  const disposedMaterials = new Set<THREE.Material>();
  const disposedTextures = new Set<THREE.Texture>();

  root.traverse(child => {
    const mesh = child as Partial<THREE.Mesh>;

    if (mesh.geometry && !disposedGeometries.has(mesh.geometry)) {
      mesh.geometry.dispose();
      disposedGeometries.add(mesh.geometry);
    }

    if (mesh.material) {
      const materials = Array.isArray(mesh.material) ? mesh.material : [mesh.material];
      for (const material of materials) {
        if (disposedMaterials.has(material)) {
          continue;
        }
        disposeMaterialTextures(material, disposedTextures);
        material.dispose();
        disposedMaterials.add(material);
      }
    }
  });
}

function disposeMaterialTextures(material: THREE.Material, disposed: Set<THREE.Texture>): void {
  const properties = material as unknown as Record<string, unknown>;

  for (const key of Object.keys(properties)) {
    const value = properties[key];
    if (value instanceof THREE.Texture && !disposed.has(value)) {
      value.dispose();
      disposed.add(value);
    }
  }
}

export type Vector3Tuple = [number, number, number];

export interface SceneManifest {
  version: number;
  modelUrl: string;
  modelScale?: Vector3Tuple;
  modelPosition?: Vector3Tuple;
  modelRotation?: Vector3Tuple;
  interactiveObjects: SceneInteractiveObject[];
  cameraTargets: Record<string, SceneCameraTarget>;
}

export interface SceneInteractiveObject {
  id: string;
  objectName: string;
  type: 'about' | 'projects' | 'skills' | 'contact';
  cameraTarget?: string;
}

export interface SceneCameraTarget {
  position: Vector3Tuple;
  lookAt: Vector3Tuple;
}

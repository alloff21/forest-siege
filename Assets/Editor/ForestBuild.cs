using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;
public static class ForestBuild {
 [MenuItem("Forest Siege/Create Scene")]
 public static void Create(){Directory.CreateDirectory("Assets/Scenes");var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);EditorSceneManager.SaveScene(scene,"Assets/Scenes/Forest.unity");EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/Forest.unity",true)};}
 [MenuItem("Forest Siege/Build WebGL")]
 public static void Build(){Create();PlayerSettings.productName="Forest Siege";PlayerSettings.WebGL.compressionFormat=WebGLCompressionFormat.Disabled;PlayerSettings.WebGL.threadsSupport=false;var report=BuildPipeline.BuildPlayer(new[]{"Assets/Scenes/Forest.unity"},"Build/WebGL",BuildTarget.WebGL,BuildOptions.None);if(report.summary.result!=BuildResult.Succeeded)throw new System.Exception("Build failed");}
}

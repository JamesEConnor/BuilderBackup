/* 
 * Copyright 2018 James Connor
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
 * WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;
using UnityEditor;

public class Builder : EditorWindow
{
	public bool shouldDelete;

	void OnGUI()
	{
		EditorWindow.GetWindow (typeof(Builder), false, this.titleContent.text, true);

		if (EditorApplication.isPlayingOrWillChangePlaymode)
			Close ();
		else if (EditorApplication.isCompiling)
			shouldDelete = true;
		else if (shouldDelete)
			Close ();

		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();

		GUILayout.Label ("Build Current Project?");

		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();

		if (GUILayout.Button ("Yes"))
			Autosave ();

		if (GUILayout.Button ("No"))
			Close ();

		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
	}

	public void Autosave()
	{
		Debug.Log ("Running...");

		BuildPlayerOptions bpo = new BuildPlayerOptions ();

		if (Wow.Is64BitOperatingSystem) 
		{
			if (Application.platform == RuntimePlatform.WindowsEditor) 
			{
				bpo.target = BuildTarget.StandaloneWindows64;
			}
			else if (Application.platform == RuntimePlatform.LinuxEditor) 
			{
				bpo.target = BuildTarget.StandaloneLinux64;
			}
			else if (Application.platform == RuntimePlatform.OSXEditor)
			{
				bpo.target = BuildTarget.StandaloneOSXIntel64;
			}
		}
		else
		{
			if (Application.platform == RuntimePlatform.WindowsEditor) 
			{
				bpo.target = BuildTarget.StandaloneWindows;
			}
			else if (Application.platform == RuntimePlatform.LinuxEditor) 
			{
				bpo.target = BuildTarget.StandaloneLinux;
			}
			else if (Application.platform == RuntimePlatform.OSXEditor)
			{
				bpo.target = BuildTarget.StandaloneOSXIntel;
			}
		}

		string[] split = Application.streamingAssetsPath.Split (new char[]{ '/' , '\\'});

		string path = "";

		for (int a = 0; a < split.Length - 2; a++) 
			path += split [a] + "/";
		path += "Builds/Backup";

		if (!System.IO.Directory.Exists (path))
			System.IO.Directory.CreateDirectory (path);

		path += "/Autosave.exe";

		if (System.IO.File.Exists (path))
			System.IO.File.Delete (path);

		if (System.IO.Directory.Exists (path.Substring (0, path.Length - 4) + "_Data")) 
		{
			ClearDirectory (path.Substring (0, path.Length - 4) + "_Data");
			System.IO.Directory.Delete (path.Substring (0, path.Length - 4) + "_Data");
		}

		bpo.locationPathName = path;
		bpo.targetGroup = BuildTargetGroup.Standalone;

		string newPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
		newPath += "/Library/ScriptAssemblies/Assembly-CSharp-Editor.dll";

		if (File.Exists (path.Substring (0, path.LastIndexOf ("/")) + "/Assembly-CSharp-Editor.dll"))
			File.Delete (path.Substring (0, path.LastIndexOf ("/")) + "/Assembly-CSharp-Editor.dll");

		File.Copy (newPath, path.Substring (0, path.LastIndexOf ("/")) + "/Assembly-CSharp-Editor.dll");

		string s = BuildPipeline.BuildPlayer (bpo);

		if (s != "")
			Debug.LogError ("An error occurred while building the player:\n" + s);
		else
			Debug.Log ("Completed build. Saved at " + bpo.locationPathName);

		Close ();
	}

	void ClearDirectory(string name)
	{
		foreach (string f in Directory.GetFiles(name))
			File.Delete (f);

		foreach (string d in Directory.GetDirectories(name)) 
		{
			ClearDirectory (d);

			Directory.Delete (d);
		}
	}
}

public static class Wow
{
	public static bool Is64BitProcess
	{
		get { return System.IntPtr.Size == 8; }
	}

	public static bool Is64BitOperatingSystem
	{
		get
		{
			if (Is64BitProcess)
				return true;
			
			bool isWow64;
			return ModuleContainsFunction("kernel32.dll", "IsWow64Process") && IsWow64Process(GetCurrentProcess(), out isWow64) && isWow64;
		}
	}

	static bool ModuleContainsFunction(string moduleName, string methodName)
	{
		System.IntPtr hModule = GetModuleHandle(moduleName);
		if (hModule != System.IntPtr.Zero)
			return GetProcAddress(hModule, methodName) != System.IntPtr.Zero;
		return false;
	}

	[DllImport("kernel32.dll", SetLastError=true)]
	[return:MarshalAs(UnmanagedType.Bool)]
	extern static bool IsWow64Process(System.IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] out bool isWow64);
	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError=true)]
	extern static System.IntPtr GetCurrentProcess();
	[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	extern static System.IntPtr GetModuleHandle(string moduleName);
	[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError=true)]
	extern static System.IntPtr GetProcAddress(System.IntPtr hModule, string methodName);
}

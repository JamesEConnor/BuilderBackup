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
using UnityEngine;
using UnityEditor;

public class WindowCreator
{
	[MenuItem("Tools/Autosave %a")]
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void Build()
	{
		Builder b = EditorWindow.CreateInstance (typeof(Builder)) as Builder;
		b.titleContent = new GUIContent ("Autosave");
		float width = 200.0f;
		float height = 40.0f;
		b.position = new Rect ((Screen.width / 2) - (width), (Screen.height / 2) - (height), width, height);
		b.minSize = new Vector2 (width, height);
		b.maxSize = new Vector2 (width, height);
		b.shouldDelete = false;
		b.Show (true);
		//b.position = new Rect(Screen.width/2, Screen.height/2, width, height);
	}
}
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RocketWorks.Config
{
	public class CreateSOMenu
	{
		private GenericMenu menu;
			
		private Type[] types;

		private UnityEditor.GenericMenu.MenuFunction2 OnTypeChoose;

		public CreateSOMenu(Type baseType, UnityEditor.GenericMenu.MenuFunction2 OnTypeChoose)
		{
			types = baseType.Assembly.GetTypes().Where(t => !t.IsAbstract && (t == baseType || t.IsSubclassOf(baseType))).ToArray();

			this.OnTypeChoose = OnTypeChoose;
			
			menu = new GenericMenu();

			foreach (var type in types)
			{
				var prePend = "";
				if (type != baseType && baseType != type.BaseType)
				{
					if (type.BaseType.IsGenericType)
						prePend += type.BaseType.GetGenericTypeDefinition().Name;
					else
						prePend += type.BaseType.Name;
					prePend += "/";
				}

				menu.AddItem(new GUIContent($"{prePend}{type.Name}"), false, OnTypeChoose, type);
			}
		}

		public void Show()
		{
			if (types.Length == 1)
			{
				OnTypeChoose(types[0]);
				return;
			}
			menu.ShowAsContext();
		}
	}
}

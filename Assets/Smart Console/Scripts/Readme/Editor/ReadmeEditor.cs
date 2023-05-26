using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ED.SC.Editor
{
    [CustomEditor(typeof(Readme))]
    [InitializeOnLoad]
    public class ReadmeEditor : UnityEditor.Editor
    {
		private Readme m_Readme;
		private bool m_Initialized;

		private static string k_ShowedReadmeSessionStateName = "ReadmeEditor.showedReadme";
		private static float k_Space = 16f;

		private GUIStyle LinkStyle => m_LinkStyle;
		[SerializeField] private GUIStyle m_LinkStyle;

		private GUIStyle TitleStyle => m_TitleStyle;
		[SerializeField] private GUIStyle m_TitleStyle;

		private GUIStyle HeadingStyle => m_HeadingStyle;
		[SerializeField] private GUIStyle m_HeadingStyle;

		private GUIStyle BodyStyle => m_BodyStyle;
		[SerializeField] private GUIStyle m_BodyStyle;

	    static ReadmeEditor()
	    {
		    EditorApplication.delayCall += SelectReadmeAutomatically;
	    }

        private static void SelectReadmeAutomatically()
        {
            if (!SessionState.GetBool(k_ShowedReadmeSessionStateName, false))
            {
                var readme = SelectReadme();
                SessionState.SetBool(k_ShowedReadmeSessionStateName, true);

                if (readme != null && !readme.loadedLayout)
                {
	                LoadLayout();
	                readme.loadedLayout = true;
                }
            }
        }

        private static void LoadLayout()
        {
            var assembly = typeof(EditorApplication).Assembly;
            var windowLayoutType = assembly.GetType("UnityEditor.WindowLayout", true);
            var method = windowLayoutType.GetMethod("LoadWindowLayout", BindingFlags.Public | BindingFlags.Static);
            method.Invoke(null, new object[] { Path.Combine(Application.dataPath, "TutorialInfo/Layout.wlt"), false });
        }

		[MenuItem("Tools/Smart Console/Getting Started", false, 0)]
        private static Readme SelectReadme()
        {
			Readme readmeObject = AssetDatabase.LoadAssetAtPath<Readme>("Assets/Smart Console/README.asset");
            
            if (!readmeObject)
            {
				return null;
			}

			Selection.objects = new Object[] { readmeObject };

			return readmeObject;
		}
        
        private void OnEnable()
        {
            if (target == null)
            {
                return;
            }
            
            m_Readme = (Readme)target;
        }

        protected override void OnHeaderGUI()
        {
            Init();
		
            var iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth/3f - 20f, 128f);
		
            GUILayout.BeginHorizontal("In BigTitle");
            {
                GUILayout.Label(m_Readme.icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));

				GUILayout.BeginVertical();
				GUILayout.Space(25);
				GUILayout.Label(m_Readme.title, TitleStyle);
				GUILayout.EndVertical();
			}
            GUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI()
        {
            Init();

            foreach (var section in m_Readme.sections)
            {
                if (!string.IsNullOrEmpty(section.heading))
                {
                    GUILayout.Label(section.heading, HeadingStyle);
                }
                
                if (!string.IsNullOrEmpty(section.text))
                {
                    GUILayout.Label(section.text, BodyStyle);
                }
                
                if (!string.IsNullOrEmpty(section.linkText))
                {
                    if (LinkLabel(new GUIContent(section.linkText)))
                    {
                        Application.OpenURL(section.url);
                    }
                }

                GUILayout.Space(k_Space);
            }
        }

        private void Init()
        {
            if (m_Initialized)
            {
                return;
            }
                
            m_BodyStyle = new GUIStyle(EditorStyles.label);
            m_BodyStyle.wordWrap = true;
            m_BodyStyle.fontSize = 14;
		
            m_TitleStyle = new GUIStyle(m_BodyStyle);
            m_TitleStyle.fontSize = 26;
		
            m_HeadingStyle = new GUIStyle(m_BodyStyle);
            m_HeadingStyle.fontSize = 18 ;
		
            m_LinkStyle = new GUIStyle(m_BodyStyle);
            m_LinkStyle.wordWrap = false;
            // Match selection color which works nicely for both light and dark skins
            m_LinkStyle.normal.textColor = new Color (0x00/255f, 0x78/255f, 0xDA/255f, 1f);
            m_LinkStyle.stretchWidth = false;
		
            m_Initialized = true;
        }

        private bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
        {
            var position = GUILayoutUtility.GetRect(label, LinkStyle, options);

            Handles.BeginGUI();
            Handles.color = LinkStyle.normal.textColor;
            Handles.DrawLine (new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
            Handles.color = Color.white;
            Handles.EndGUI();

            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

            return GUI.Button(position, label, LinkStyle);
        }
    }
}

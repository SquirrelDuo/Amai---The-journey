#if UNITY_EDITOR
#define P128

using System;
using System.Xml.Serialization;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;

#if P128
namespace EModules.PostPresets128
#else
namespace EModules.PostPresets1000
#endif
{

  //! Supported 'Unity PostProcessing' , 'AmplifyColor'
  interface ISupportedPostComponent
  {
    //!  GUI  //
    void LeftSideGUI(EditorWindow window, float leftwidth); // postrocessing compnent gui
    void TopFastButtonsGUI(EditorWindow window, float leftwidth); // buttons to enable/disable effects

    //!  Params  //
    string GetHashString(); // hash depends settings
    bool IsValid { get; } // is a component available for use
    MonoBehaviour MonoComponent { get; set; } // is a component enable in inspector
    Type MonoComponentType { get; }
    bool AntiAliasEnable { get; set; } // 
    bool LutEnable { get; set; } // 
    Texture2D LutTexture { get; set; } // 
    float LutAmount { get; set; } // LUT opacity value

    //!  Utills  //
    void CREATE_UNDO(string undoName); // create undo
    void SET_DIRTY(); // save after change
    void CameraPredrawAction(); // to restore default unity postprocessing antialiasing
    void CameraPostDrawAction(); // to restore default unity postprocessing antialiasing
  }

  [XmlRoot( "GradiendDataCollection" )]
  public class GradiendDataArray
  {
    public List<GradiendData> list;
  }
  public struct GradiendData
  {
    public string name;
    public double bright;
    public double hue;
    public double compareToFirst;
    [XmlIgnore]
    public Texture2D gradientTexture;
  }

  public class Params
  {
#if P128
    public static Type WindowType = typeof(PostPresets128Window);
    public const string TITLE = "Post 128+";
#else
    public static Type WindowType = typeof(PostPresets1000Window);
    public const string TITLE = "Post 1000+";
#endif

    public static GUIStyle Label;
    public static GUIStyle Button;

    public static CachedFloat AutoRefresh = new CachedFloat("AutoRefresh", 1);
    public static CachedFloat scrollX = new CachedFloat("ScrollX");
    public static CachedFloat scrollY = new CachedFloat("ScrollY");
    public static CachedFloat presetScrollX = new CachedFloat("presetScrollX");
    public static CachedFloat presetScrollY = new CachedFloat("presetScrollY");
    public static CachedFloat favScrollX = new CachedFloat("favScrollX");
    public static CachedFloat favScrollY = new CachedFloat("favScrollY");
    public static CachedString filtres = new CachedString("filtres", "");
    public static CachedFloat sortMode = new CachedFloat("sortMode", 0);
    public static CachedFloat sortInverse = new CachedFloat("sortInverse", 0);
    public static CachedFloat zoomFactor = new CachedFloat("zoomFactor", 0);
    public static CachedFloat showFav = new CachedFloat("showFav", 0);

    public static Vector2 scroll;

    public static Type PostProcessingBehaviourType = null;
    public static Type PostProcessingProfileType = null;
    public static Type AmplifyBaseType = null;
    public static Type AmplifyEffectType = null;

    public static Rect Shrink(Rect r, int value)
    {
      r.x += value;
      r.y += value;
      r.width -= value * 2;
      r.height -= value * 2;
      return r;
    }

    static  GUIContent  _mGUIContent = new GUIContent();
    public static GUIContent CONTENT(string text, string tooltip)
    {
      _mGUIContent.text = text;
      _mGUIContent.tooltip = tooltip;
      return _mGUIContent;
    }

    static string _mEditorResourcesPath;
    public static string EditorResourcesPath
    {
      get {
        if (string.IsNullOrEmpty( _mEditorResourcesPath ))
        {
          string path;

          if (searchForEditorResourcesPath( out path ))
            _mEditorResourcesPath = path;
          else
          {
            EditorUtility.DisplayDialog( "PostPresetsWindow Instalation Error", "Unable to locate editor resources. Make sure the PostPresetsWindow package has been installed correctly.", "Ok" );
            return null;
          }
        }
        return _mEditorResourcesPath;
      }
    }

    static bool searchForEditorResourcesPath(out string path)
    {
      var allPathes = AssetDatabase.GetAllAssetPaths();
      path = allPathes.FirstOrDefault( p => p.EndsWith( WindowType.Name + ".cs" ) );
      if (string.IsNullOrEmpty( path )) return false;
      var tempPath = path.Remove( path.LastIndexOf( '/' ) );
      var candidates = allPathes.Where( p => p.StartsWith( tempPath ) );
      path = tempPath;
      if (candidates.Any( c => c.Contains( "ColorGradients" ) )) return true;
      tempPath = path.Remove( path.LastIndexOf( '/' ) );
      candidates = allPathes.Where( p => p.StartsWith( tempPath ) );
      path = tempPath;
      if (candidates.Any( c => c.Contains( "ColorGradients" ) )) return true;
      return false;
    }
  }




  ///////////////////////
  ///////////////////////
  ///////////////////////
  //! EditorWindow *** //
  ///////////////////////
  ///////////////////////
  ///////////////////////

#if P128
  public class PostPresets128Window : EditorWindow
#else
  public class PostPresets1000Window : EditorWindow
#endif

  {


    const int IMAGE_RENDER_PER_FRAME = 3;

#if !P128
    public Texture2D favicon_enable = null,favicon_disable = null;
#endif
    static EditorWindow currentWindow;
    public Texture2D[] t;
    static Texture2D[] _mGradients;
    public static Texture2D[] gradients
    {
      get {
        if (string.IsNullOrEmpty( Params.EditorResourcesPath )) return new Texture2D[0];
        if (_mGradients == null) _mGradients = GetGradients( Params.EditorResourcesPath );
        return _mGradients;
      }
    }


    public static Texture2D[] GetGradients(string path)
    {
      path = path + "/ColorGradients";
      return AssetDatabase.GetAllAssetPaths()
       .Where( p => p.StartsWith( path ) )
       .Select( p => AssetDatabase.LoadAssetAtPath( p, typeof( Texture2D ) ) as Texture2D )
       .Where( t => t )
       .OrderBy( t => t.name )
       .ToArray();
    }


    [MenuItem( Params.TITLE + "/Presets Manager", false, 0 )]
    public static void Init()
    {
      if (string.IsNullOrEmpty( Params.EditorResourcesPath )) return;

      _mGradients = null;
      foreach (var window in Resources.FindObjectsOfTypeAll( Params.WindowType )) ((EditorWindow)window).Close();

      currentWindow = GetWindow( Params.WindowType, false, Params.TITLE, true );
      if (!EditorPrefs.GetBool( "EModules/" + Params.TITLE + "/Init", false ))
      {
        EditorPrefs.GetBool( "EModules/" + Params.TITLE + "/Init", true );
        var p  =currentWindow.position;
        p.width = 1100;
        p.height = 650;
        p.x = Screen.currentResolution.width / 2 - p.width / 2;
        p.y = Screen.currentResolution.height / 2 - p.height / 2;
        currentWindow.position = p;
      }
      var k = "EModules/" + Params.TITLE + "/ScreenShot";
      foreach (var texture2D in Resources.FindObjectsOfTypeAll<Texture2D>().Where( t => t.name == k ))
        DestroyImmediate( texture2D, true );

      Params.WindowType.GetMethod( "ResetWindow", (System.Reflection.BindingFlags)int.MaxValue ).Invoke( currentWindow, null );

      Undo.undoRedoPerformed -= UndoPerform;
      Undo.undoRedoPerformed += UndoPerform;
    }

    [MenuItem( Params.TITLE + "/Download Unity 'Post Processing Stack'", false, 100 )]
    public static void OpenURL_UNI()
    {
      Application.OpenURL( "https://www.assetstore.unity3d.com/#!/content/83912" );
    }
    [MenuItem( Params.TITLE + "/Download 'Amplify Color'", false, 101 )]
    public static void OpenURL_AMP()
    {
      Application.OpenURL( " https://www.assetstore.unity3d.com/en/#!/content/1894" );
    }




    GradiendDataArray gradientDataArray = new GradiendDataArray(){list = new List<GradiendData>() };

    void ResetWindow()
    {
      mayResetScroll = true;
      renderedScreen = new Texture2D[0];
      renderedDoubleCheck = new Texture2D[0];
      EditorPrefs.SetInt( "EModules/" + Params.TITLE + "/Scene", SceneManager.GetActiveScene().GetHashCode() );

      var monoType = typeof(MonoBehaviour);
      var scType = typeof(ScriptableObject);
      var types =Params.WindowType.Assembly.GetTypes();
      Params.PostProcessingBehaviourType = types.FirstOrDefault( t => (t.Name.EndsWith( ".PostProcessingBehaviour" ) || t.Name == "PostProcessingBehaviour") && monoType.IsAssignableFrom( t ) );
      if (Params.PostProcessingBehaviourType != null)
      {
        Params.PostProcessingProfileType = types.FirstOrDefault( t => (t.Name.EndsWith( ".PostProcessingProfile" ) || t.Name == "PostProcessingProfile") && scType.IsAssignableFrom( t ) );
        if (Params.PostProcessingProfileType == null)
        {
          Debug.LogWarning( "Missing PostProcessingProfileType" );
          Params.PostProcessingBehaviourType = null;
        }
      }
      Params.AmplifyEffectType = types.FirstOrDefault( t => t.Name.EndsWith( ".AmplifyColorEffect" ) || t.Name == "AmplifyColorEffect" );
      if (Params.AmplifyEffectType != null) Params.AmplifyBaseType = Params.AmplifyEffectType.BaseType;
      else Params.AmplifyBaseType = null;



      InitializeCompareArray();
      InitializeFavorites();
    }

    // char[] trimChars = "1234567890+ ABCDEFGHJKLMNOPQRSTUWXYZ".ToCharArray();

    void InitializeCompareArray()
    {
      var path = Params.EditorResourcesPath + "/.ColorGradients.data";
      if (gradientDataArray == null) gradientDataArray = new GradiendDataArray() { list = new List<GradiendData>() };
      if (File.Exists( path ))
      {
        using (StreamReader sr = new StreamReader( path ))
        {
          while (!sr.EndOfStream)
          {
            var data = sr.ReadLine().Split(':');
            gradientDataArray.list.Add( new GradiendData()
            {
              name = data[0],
              bright = double.Parse( data[1] ),
              hue = double.Parse( data[2] ),
              compareToFirst = double.Parse( data[3] ),
            } );
          }
        }
      }

      name_to_index.Clear();
      default_gradients.Clear();
      hue_gradients.Clear();
      bright_gradients.Clear();
      compareToFirst_gradients.Clear();
      inverse_default_gradients.Clear();
      inverse_hue_gradients.Clear();
      inverse_bright_gradients.Clear();
      inverse_compareToFirst_gradients.Clear();

      for (int i = 0 ; i < gradients.Length ; i++)
      {
        name_to_index.Add( gradients[i].name, i );
        // var n = gradients[i].name.TrimEnd( trimChars );
        // name_to_filterstring.Add( gradients[i].name, n );
        // if (!filtername_to_filterindex.ContainsKey( n )) filtername_to_filterindex.Add( n, (filtername_to_filterindex.Count + 1) / 4 );

        default_gradients.Add( new GradiendData() { name = gradients[i].name, gradientTexture = gradients[i] } );
      }

      //read data for unplussed textures
      for (int i = 0 ; i < gradientDataArray.list.Count ; i++)
      {
        var data = gradientDataArray.list[i];
        if (!name_to_index.ContainsKey( data.name )) continue;
        data.gradientTexture = gradients[name_to_index[data.name]];

        hue_gradients.Add( data );
        bright_gradients.Add( data );
        compareToFirst_gradients.Add( data );
      }

      //sort unplussed textures
      hue_gradients.Sort( (a, b) => Sign( b.hue, a.hue ) );
      bright_gradients.Sort( (a, b) => Sign( b.bright, a.bright ) );
      compareToFirst_gradients.Sort( (a, b) => Sign( b.compareToFirst, a.compareToFirst ) );

      inverse_default_gradients = default_gradients.Reverse<GradiendData>().ToList();
      inverse_hue_gradients = hue_gradients.Reverse<GradiendData>().ToList();
      inverse_bright_gradients = bright_gradients.Reverse<GradiendData>().ToList();
      inverse_compareToFirst_gradients = compareToFirst_gradients.Reverse<GradiendData>().ToList();

      //add plussed textures to sorted unplussed
      /*AddPlusesTextures( ref hue_gradients );
      AddPlusesTextures( ref bright_gradients );
      AddPlusesTextures( ref compareToFirst_gradients );*/
    }

    /*void AddPlusesTextures(ref List<GradiendData> array)
    {
      for (int i = 0 ; i < array.Count ; i++)
      {
        var target = array[i].name + " +";
        while (name_to_index.ContainsKey( target ))
        {
          var t = gradients[name_to_index[target]];
          array.Insert( i, array[i] );
          i++;
          var data = array[i];
          data.gradientTexture = t;
          array[i] = data;
          target += '+';
        }
      }
    }*/

    int Sign(double a, double b)
    {
      if (a > b) return 1;
      if (a < b) return -1;
      return 0;
    }

    // Dictionary<string,int> filtername_to_filterindex = new Dictionary<string, int>();
    // Dictionary<string,string> name_to_filterstring = new Dictionary<string, string>();
    Dictionary<string,int> name_to_index = new Dictionary<string, int>();
    List<GradiendData> default_gradients = new List<GradiendData>();
    List<GradiendData> hue_gradients = new List<GradiendData>();
    List<GradiendData> bright_gradients = new List<GradiendData>();
    List<GradiendData> compareToFirst_gradients = new List<GradiendData>();
    List<GradiendData>inverse_default_gradients = new List<GradiendData>();
    List<GradiendData>inverse_hue_gradients = new List<GradiendData>();
    List<GradiendData>inverse_bright_gradients = new List<GradiendData>();
    List<GradiendData>inverse_compareToFirst_gradients = new List<GradiendData>();
    string[] sotrNames = new string[]{"Name", "Saturate", "Warming", "Content"};
    /* Dictionary<double,string> hue_to_name = new Dictionary<double,string>();
     Dictionary<double,string> bright_to_name = new Dictionary<double,string>();
     Dictionary<double,string> compareToFirst_to_name = new Dictionary<double,string>();*/


    static Scene activeScene;
    static void UndoPerform()
    {
      if (!currentWindow) currentWindow = Resources.FindObjectsOfTypeAll( Params.WindowType ).FirstOrDefault() as EditorWindow;
      if (!currentWindow) return;
      currentWindow.Repaint();
    }

    private void OnDestroy()
    {
      EditorApplication.modifierKeysChanged -= KeysChanged;
      Undo.undoRedoPerformed -= UndoPerform;
    }

    private void OnDisable()
    {
      EditorApplication.modifierKeysChanged += KeysChanged;

    }

    private void OnEnable()
    {
      EditorApplication.modifierKeysChanged -= KeysChanged;
      EditorApplication.modifierKeysChanged += KeysChanged;
    }

    void KeysChanged()
    {
      if (currentWindow != null) currentWindow.Repaint();
    }



    Dictionary<string,int> favorites = new Dictionary<string, int>();

    void CreateFavorite(string name)
    {
      if (!favorites.ContainsKey( name )) favorites.Add( name, 0 );
      StringBuilder result = new StringBuilder();
      foreach (var f in favorites)
        result.AppendLine( f.Key );
      File.WriteAllText( Params.EditorResourcesPath + "/.Favorites.data", result.ToString().TrimEnd( '\n' ) );
    }
    void RemoveFavorite(string name)
    {
      if (!favorites.ContainsKey( name )) return;
      favorites.Remove( name );
      StringBuilder result = new StringBuilder();
      foreach (var f in favorites)
        result.AppendLine( f.Key );
      File.WriteAllText( Params.EditorResourcesPath + "/.Favorites.data", result.ToString().TrimEnd( '\n' ) );
    }

    void InitializeFavorites()
    {
      favorites.Clear();
      var path = Params.EditorResourcesPath + "/.Favorites.data";
      if (File.Exists( path ))
      {
        using (StreamReader sr = new StreamReader( path ))
        {
          while (!sr.EndOfStream)
          {
            var f = sr.ReadLine();
            if (!favorites.ContainsKey( f )) favorites.Add( f, 0 );
          }
        }
      }
    }




    PostPresets_UnityPostGUI postPresets_UnityPostGUI = new PostPresets_UnityPostGUI();
    PostPresets_AmplifyPostGUI postPresets_AmplifyPostGUI = new PostPresets_AmplifyPostGUI();



    void OnGUI()
    {
      if (!SceneManager.GetActiveScene().IsValid()) return;

      if (!currentWindow)
      {
        currentWindow = Resources.FindObjectsOfTypeAll( Params.WindowType ).FirstOrDefault() as EditorWindow;
        if (currentWindow) ResetWindow();
      }
      if (!currentWindow) return;

      if (SceneManager.GetActiveScene().GetHashCode() != EditorPrefs.GetInt( "EModules/" + Params.TITLE + "/Scene", -1 )) ResetWindow();

      if (Params.Label == null)
      {
        Params.Label = new GUIStyle( GUI.skin.label );
        Params.Label.fontSize = 14;
        Params.Label.fontStyle = FontStyle.Bold;

        Params.Button = new GUIStyle( GUI.skin.button );
        // Button.fontSize = 14;
        var t = new Texture2D(1,1,TextureFormat.ARGB32,false,true);
        t.hideFlags = HideFlags.DontSave;
        t.SetPixel( 0, 0, new Color( 0, 0.1f, 0.4f, 0.3f ) );
        t.Apply();
        Params.Button.normal.background = null;
        Params.Button.hover.background = null;
        Params.Button.focused.background = null;
        Params.Button.active.background = t;
      }


      if (!Camera.main)
      {
        GUILayout.Label( "No Camera", Params.Label );
        return;
      }

      ISupportedPostComponent currentComponent = null;
      MonoBehaviour c1 = null;
      MonoBehaviour c2 = null;
      if (Params.PostProcessingBehaviourType != null)
        c1 = Camera.main.GetComponent( Params.PostProcessingBehaviourType ) as MonoBehaviour;
      if (Params.AmplifyBaseType != null)
        c2 = Camera.main.GetComponent( Params.AmplifyBaseType ) as MonoBehaviour;

      if (c1 && c1.enabled) currentComponent = postPresets_UnityPostGUI;
      else if (c2 && c2.enabled) currentComponent = postPresets_AmplifyPostGUI;
      else if (c1) currentComponent = postPresets_UnityPostGUI;
      else if (c2) currentComponent = postPresets_AmplifyPostGUI;
      else if (Params.PostProcessingBehaviourType != null) currentComponent = postPresets_UnityPostGUI;
      else if (Params.AmplifyBaseType != null) currentComponent = postPresets_AmplifyPostGUI;


      if (currentComponent == null)
      {
        GUILayout.Label( "Unity PostProcessing or AmplifyColor not imported", Params.Label );
        if (GUILayout.Button( "Download Unity 'Post Processing Stack'", GUILayout.Height( 40 ) ))
        {
          Application.OpenURL( "https://www.assetstore.unity3d.com/#!/content/83912" );
        }
        if (GUILayout.Button( "Download 'Amplify Color'", GUILayout.Height( 40 ) ))
        {
          Application.OpenURL( " https://www.assetstore.unity3d.com/en/#!/content/1894" );
        }
        return;
      }


      // Left column ////
      GUILayout.BeginHorizontal();
      var leftwidth =  Mathf.Clamp( position.width / 3f, 200, 350 );

      GUILayout.BeginVertical( GUILayout.Width( leftwidth ) );
      GUILayout.Label( "Camera: " + Camera.main.name, Params.Label );
      if (GUI.Button( GUILayoutUtility.GetLastRect(), "", Params.Button )) Selection.objects = new[] { Camera.main.gameObject };
      EditorGUIUtility.AddCursorRect( GUILayoutUtility.GetLastRect(), MouseCursor.Link );


      if (currentComponent.MonoComponent == null)
      {
        if (GUILayout.Button( "Add " + currentComponent.MonoComponentType + " Script", GUILayout.Height( 200 ) ))
        {
          Undo.RecordObject( Camera.main.gameObject, "Add " + currentComponent.MonoComponentType + " Script" );
          Camera.main.gameObject.AddComponent( currentComponent.MonoComponentType );
          EditorUtility.SetDirty( Camera.main.gameObject );
        }
        return;
      }

      if (!currentComponent.MonoComponent.enabled)
      {
        GUILayout.Label( currentComponent.MonoComponent.GetType().Name + " Component Disabled", Params.Label );
        if (GUILayout.Button( "Enable " + currentComponent.MonoComponent.GetType().Name + " Component", GUILayout.Height( 200 ) ))
        {
          Undo.RecordObject( currentComponent.MonoComponent, "Enable " + currentComponent.MonoComponent.GetType().Name + " Component" );
          currentComponent.MonoComponent.enabled = true;
          EditorUtility.SetDirty( currentComponent.MonoComponent );
          UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }
        return;
      }

      if (widthCheck == null) widthCheck = position.width;
      if (widthCheck.Value != position.width)
      {
        widthCheck = position.width;
        ClearImages( false );
        currentWindow.Repaint();
      }

      //! *** POSTPROCESSING COMPONENT GUI *** //
      currentComponent.LeftSideGUI( this, leftwidth );
      //! *** POSTPROCESSING COMPONENT GUI *** //

      if (GUILayout.Button( "http://emodules.me/", Params.Button )) Application.OpenURL( "http://emodules.me/" );
      EditorGUIUtility.AddCursorRect( GUILayoutUtility.GetLastRect(), MouseCursor.Link );

      GUILayout.EndVertical();
      // Left column

      GUILayout.Space( 10 );

      leftwidth += 20;

      //! PRESETS GRID GUI
      if (currentComponent.IsValid)
      {
        GUILayout.BeginVertical();
        ////////////////////////
        currentComponent.TopFastButtonsGUI( this, leftwidth );
        ////////////////////////
        FiltresAndSorting( this, leftwidth );
        ////////////////////////
        DrawPresets( currentComponent );
        ////////////////////////
        GUILayout.EndVertical();
      }
      //! PRESETS GRID GUI

      GUILayout.EndHorizontal();



    }//!OnGUI



    void FiltresAndSorting(EditorWindow window, float leftWidth)
    {
#if !P128
      GUI.enabled = true;
#else
      GUI.enabled = false;
#endif

      // FILTRES
      //var filtresCount = filtername_to_filterindex.Values.Max();
      /*for (int i = 0 ; i < filtresCount ; i++)
      {
        var filtMask = 1 << i;
        var enable = ((int)Params.filtres & filtMask) != 0;
        var captureI=i;
        var name = filtername_to_filterindex.Where(f=>f.Value == captureI).Select(f=>f.Key).Aggregate((a,b)=>a+'\n' +b);
        if (Button( GetRect( leftWidth, 40, window, 8 ), name, enable ))
        {
          if (enable) Params.filtres.Set( ((int)Params.filtres & ~filtMask) );
          else Params.filtres.Set( ((int)Params.filtres | filtMask) );
          renderedDoubleCheck = new Texture2D[gradients.Length];
          window.Repaint();
        }
      }*/
      GUILayout.BeginHorizontal();
      var lineR=  GetRect( leftWidth, 16, window, 1 ); //SPACE
      GUILayout.EndHorizontal();
      var seaR = lineR;
      seaR.width *= 0.7f;
      lineR.x += seaR.width + 40;
      lineR.width -= seaR.width + 40;
      var newFilter = EditorGUI.TextField(seaR, "Search: ", (string)Params.filtres);
      if (newFilter != Params.filtres)
      {
        Params.filtres.Set( newFilter );
        window.Repaint();
      }
      seaR.x += seaR.width;
      seaR.width = 20;
      if (GUI.Button( seaR, "X" ))
      {
        Params.filtres.Set( "" );
        EditorGUI.FocusTextInControl( null );
        ClearImages( true );
        window.Repaint();
      }
      EditorGUIUtility.AddCursorRect( lineR, MouseCursor.Link );
      if (GUI.Button( lineR, "Show Favorites", Params.Button ))
      {
        Params.showFav.Set( 1 - Params.showFav );
        ClearImages( true );
        if (Params.showFav == 0) mayResetScroll = WasElementsChanged;
        WasElementsChanged = false;
        window.Repaint();
      }
      if (Params.showFav == 1) EditorGUI.DrawRect( lineR, new Color( 0.8f, 0.6f, 0.33f, 0.4f ) );
      lineR.x += lineR.width;
      lineR.width = 16;
#if !P128
      GUI.DrawTexture( lineR, favicon_enable );
#endif
      // FILTRES


      // SORTING
      GUILayout.BeginHorizontal();
      var D = 8;
      var r = GetRect( leftWidth, 20, window ,D);
      GUI.Label( r, "Sorting:" );
      for (int i = 0 ; i < 4 ; i++)
      {
        var sortNotInverse = ((int)Params.sortInverse & (1 << i)) == 0;
        if (Button( GetRect( leftWidth, 20, window, D ), sotrNames[i] + (Params.sortMode == i ? (sortNotInverse ? "▼" : "▲") : ""), Params.sortMode == i, "Set Ordering Method" ))
        {
          if (Params.sortMode == i)
          {
            if (!sortNotInverse) Params.sortInverse.Set( ((int)Params.sortInverse & ~(1 << i)) );
            else Params.sortInverse.Set( ((int)Params.sortInverse | (1 << i)) );
          }
          else
          {
            Params.sortMode.Set( (float)i );
          }
          ClearImages( true );
          window.Repaint();
        }
      }

      GUI.enabled = true;

      GetRect( leftWidth, 20, window, D ); //SPACE
      GUI.Label( GetRect( leftWidth, 20, window, D ), "Zoom:" );

      var ZoomRect= GetRect( leftWidth, 20, window, D );
      ZoomRect.width /= 3;
      for (int i = 0 ; i < 3 ; i++)
      {
        if (Button( ZoomRect, "x" + (i + 1) * 3, Params.zoomFactor == i, "Set Zooming Factor" ))
        {
          Params.zoomFactor.Set( i );
          ClearImages( true );
          window.Repaint();
        }
        ZoomRect.x += ZoomRect.width;
      }
      GUILayout.EndHorizontal();
      // SORTING



    }//!FiltresAndSorting



    Rect GetRect(float leftWidth, float height, EditorWindow window, int divider)
    {
      return EditorGUILayout.GetControlRect( GUILayout.Width( Math.Min( (window.position.width - leftWidth - 10 - 40) / divider, 700 / divider ) ), GUILayout.Height( height ) );
    }

    bool Button(Rect rect, string name, bool enable, string helpTExt = null)
    {
      if (enable) EditorGUI.DrawRect( rect, new Color( 0.33f, 0.6f, 0.8f, 0.4f ) );
      EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
      var content = Params.CONTENT( name, helpTExt ?? "enable/disable " + name.Replace( '\n', ' ' ) );
      var result= (GUI.Button( rect, content ));
      //if (enable) EditorGUI.DrawRect( rect, new Color( 0.33f, 0.6f, 0.8f, 0.4f ) );
      if (Event.current.type == EventType.Repaint && enable) GUI.skin.button.Draw( rect, content, true, true, true, true );
      if (enable) EditorGUI.DrawRect( rect, new Color( 0.33f, 0.6f, 0.8f, 0.4f ) );
      return result;
    } //! TOP FAST BUTTIONS 


    float? widthCheck;

    Texture2D[] renderedDoubleCheck = new Texture2D[0];
    Texture2D[] renderedScreen = new Texture2D[0];
    static int renderedIndex;
    static bool mayResetScroll = true;
    static string oldCheck;

    bool SeartchValidate(string currentName)
    {
      //if (((1 << filtername_to_filterindex[name_to_filterstring[targetArray[i].name]]) & (int)Params.filtres) == 0) continue;
      return string.IsNullOrEmpty( Params.filtres ) || currentName.ToLower().Contains( ((string)Params.filtres).ToLower() );
    }

    void ClearImages(bool ClearCache)
    {
      renderedIndex = 0;
      if (ClearCache)
      {
        foreach (var t in renderedScreen) if (t) DestroyImmediate( t, true );
        mayResetScroll = true;
        renderedScreen = new Texture2D[0];
        renderedDoubleCheck = new Texture2D[0];
      }
      else
      {
        renderedDoubleCheck = new Texture2D[renderedScreen.Length];
      }
    }

    bool WasElementsChanged = false;

    void DrawPresets(ISupportedPostComponent component)
    {
      var zoom = (int)Mathf.Clamp(Params.zoomFactor +1,1,3);
      int cellHeight = (int)((125 + 16) / ((zoom - 1)/2f + 1));
      int CAPTUREHeight = (int)((125 ) / ((zoom - 1)/2f + 1));
      var WIN_HEIGHT = position.height - (40 + 20 + 24 + 20 );


      var sortNotInverse = ((int)Params.sortInverse & (1 << (int)Params.sortMode)) == 0;
      var targetArray = default_gradients;
      switch ((int)Params.sortMode)
      {
        case 0: targetArray = sortNotInverse ? default_gradients : inverse_default_gradients; break;
        case 1: targetArray = sortNotInverse ? bright_gradients : inverse_bright_gradients; break;
        case 2: targetArray = sortNotInverse ? hue_gradients : inverse_hue_gradients; break;
        case 3: targetArray = sortNotInverse ? compareToFirst_gradients : inverse_compareToFirst_gradients; break;
      }//sort type

      var l = component.LutTexture;
      int selectIndex = -1;
      int filteredSelectIndex = -1;
      var __i  = -1;
      var __M  = -1;
      for (__i = 0 ; __i < targetArray.Count ; __i++)
      {
        if (!SeartchValidate( targetArray[__i].name )) continue;
        if (Params.showFav == 1 && !favorites.ContainsKey( targetArray[__i].name )) continue;
        __M++;
        if (selectIndex == -1 && l == targetArray[__i].gradientTexture)
        {
          filteredSelectIndex = __M;
          selectIndex = __i;
        }
      }

      // var XXX = Math.Min(3 * zoom, Math.Max(1, M + 1));
      var XXX = 3 * zoom;
      int displaingCount = Mathf.CeilToInt(WIN_HEIGHT / cellHeight + 1) * XXX;

      var scrollX = Params.presetScrollX;
      var scrollY = Params.presetScrollY;
      if (Params.showFav == 1)
      {
        scrollX = Params.favScrollX;
        scrollY = Params.favScrollY;
      }

      if (mayResetScroll)
      {
        mayResetScroll = false;
        if (filteredSelectIndex != -1)
        {
          var fixedcellHeight = cellHeight;
          //(float)Params.presetScrollY + " " + filteredSelectIndex / XXX * cellHeight + " " + WIN_HEIGHT + " " + (filteredSelectIndex / XXX * cellHeight +
          //Debug.Log( fixedcellHeight > Params.presetScrollY + WIN_HEIGHT );
          if (filteredSelectIndex / XXX * fixedcellHeight < scrollY) scrollY.Set( filteredSelectIndex / XXX * fixedcellHeight );
          else if (filteredSelectIndex / XXX * fixedcellHeight + fixedcellHeight > scrollY + WIN_HEIGHT) scrollY.Set( filteredSelectIndex / XXX * fixedcellHeight + fixedcellHeight - WIN_HEIGHT );
        }
      }//frame scroll


      Params.scroll.x = scrollX;
      Params.scroll.y = scrollY;
      Params.scroll = GUILayout.BeginScrollView( Params.scroll );
      scrollX.Set( Params.scroll.x );
      scrollY.Set( Params.scroll.y );



      GUILayout.BeginVertical();
      var height = Mathf.Ceil( gradients.Length / (float)XXX);
      int wasRender = 0;
      int __FirstRenderInc = -1;
      bool needRepaint = false;

      var newCheck = component.GetHashString();
      if (Params.AutoRefresh == 1 && !string.Equals( newCheck, oldCheck, StringComparison.Ordinal ))
      {
        oldCheck = newCheck;
        ClearImages( false );
        currentWindow.Repaint();

      }//refrest if changed



      __i = -1;
      Rect line = new Rect();
      bool renderTexture = false;
      bool newLine = false;

      if (__M == -1)
      {
        if (Params.showFav == 1) GUILayout.Label( "No Favorite Elements", Params.Label );
        else GUILayout.Label( "No elements '" + (string)Params.filtres + "' found", Params.Label );
      }
      else
      {
        line = EditorGUILayout.GetControlRect( GUILayout.Height( (__M / XXX + 1) * cellHeight ) );
        line.height = cellHeight;
        line.y -= line.height;
        int __renderInc = -1;

        for (int y = 0 ; y < height ; y++)
        {
          newLine = true;


          for (int x = 0 ; x < XXX ; x++)
          {
            __i++;
            //var i = x + y * XXX;
            if (__i >= targetArray.Count) break;

            if (!SeartchValidate( targetArray[__i].name ))
            {
              x--;
              continue;
            }
            if (Params.showFav == 1 && !favorites.ContainsKey( targetArray[__i].name ))
            {
              x--;
              continue;
            }

            if (newLine)
            {
              newLine = false;
              //line = EditorGUILayout.GetControlRect( GUILayout.Height( cellHeight ) );
              line.y += line.height;
              renderTexture = line.y + line.height >= Params.scroll.y && line.y - line.height <= Params.scroll.y + WIN_HEIGHT;
            }



            var rect = line;
            rect.width = line.width / XXX;
            rect.x = rect.width * x;

            if (!renderTexture) continue;
            __renderInc++;

            if (__FirstRenderInc == -1) __FirstRenderInc = __renderInc;

            if (renderedScreen.Length != targetArray.Count)
            {
              System.Array.Resize( ref renderedScreen, targetArray.Count );
              ClearImages( false );
              currentWindow.Repaint();
            }

            if (renderedIndex < __FirstRenderInc) renderedIndex = __FirstRenderInc;

            /* if (Params.showFav == 1)
               Debug.Log( (wasRender < (Params.zoomFactor + 1) * IMAGE_RENDER_PER_FRAME) + " " + (renderedIndex % displaingCount) + " " + __renderInc + " " + __FirstRenderInc );*/

            if (wasRender < (Params.zoomFactor + 1) * IMAGE_RENDER_PER_FRAME && renderedIndex % displaingCount == (__renderInc - __FirstRenderInc))
            {
              if ((int)rect.width - 10 > 0 && !renderedDoubleCheck[__i])
              {
                renderedIndex++;

                component.CameraPredrawAction(); // PRE

                //memory
                var oldLutEnable = component.LutEnable;
                var oldLutTexture = component.LutTexture;
                var oldAmount = component.LutAmount;
                //change
                component.LutEnable = true;
                component.LutTexture = (targetArray[__i]).gradientTexture;
                component.LutAmount = (1);

                var oldAnti = component.AntiAliasEnable;
                component.AntiAliasEnable = false;
                //draw
                if (renderedScreen[__i]) DestroyImmediate( renderedScreen[__i], true );
                renderedDoubleCheck[__i] = renderedScreen[__i] = TakeScreen( Camera.main, (int)rect.width - 10, CAPTUREHeight );
                component.AntiAliasEnable = oldAnti;

                //restore
                component.LutEnable = oldLutEnable;
                component.LutTexture = (oldLutTexture);
                component.LutAmount = (oldAmount);

                component.CameraPostDrawAction(); // POST

                wasRender++;
              }
            }

            var screen =  renderedScreen[__i];

            if (!renderedDoubleCheck[__i]) needRepaint = true;

            if (DrawCell( rect, screen, targetArray[__i].name, selectIndex == __i, component ))
            {
              WasElementsChanged = true;
              component.CREATE_UNDO( "set " + targetArray[__i].name );
              component.LutTexture = (targetArray[__i].gradientTexture);
              if (!component.LutEnable) component.LutAmount = (1);
              component.LutEnable = true;
              component.SET_DIRTY();
              UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
          }
        }
      }


      GUILayout.EndVertical();


      if (wasRender < (Params.zoomFactor + 1) * IMAGE_RENDER_PER_FRAME)
      {
        // renderedIndex = firstI;
        renderedIndex += (int)(Params.zoomFactor + 1) * IMAGE_RENDER_PER_FRAME - wasRender;
        //needRepaint = true;
      }
      if (needRepaint)
      {
        Repaint();
      }
      GUILayout.EndScrollView();
    }//!DrawPresets


    bool DrawCell(Rect rect, Texture2D tex, string name, bool selected, ISupportedPostComponent component)
    {
      rect = Params.Shrink( rect, 2 );
      if (Event.current.type == EventType.repaint) GUI.skin.window.Draw( rect, new GUIContent(), 0 );
      rect = Params.Shrink( rect, 2 );

      if (selected) EditorGUI.DrawRect( rect, Color.red );

      var tr = rect;
      var lr = tr;
      tr.height -= 16;
      tr = Params.Shrink( tr, 2 );
      if (tex) GUI.DrawTexture( tr, tex );
      else GUI.DrawTexture( tr, Texture2D.blackTexture );

      lr.y += lr.height -= 16;
      lr.height = 16;
      GUI.Label( lr, name );

      if (selected)
      {
        lr.width /= 2;
        lr.x += lr.width;
        lr.width -= 10;
        var newAmount = GUI.HorizontalSlider( lr, component.LutAmount,  0,1);
        if (newAmount != component.LutAmount)
        {
          component.CREATE_UNDO( "set post blend" );
          component.LutAmount = newAmount;
          component.SET_DIRTY();
          SceneView.RepaintAll();
        }
      }

#if !P128
      var favcontains = favorites.ContainsKey( name );
      var fav_r = rect;
      var SIZE = Math.Min(32,fav_r.height* 0.3f);
      fav_r.x += fav_r.width - SIZE - 5;
      fav_r.y += 5;
      fav_r.width = fav_r.height = SIZE;
      if (favcontains || selected || Event.current.control)
      {
        GUI.DrawTexture( fav_r, favorites.ContainsKey( name ) ? favicon_enable : favicon_disable );
        EditorGUIUtility.AddCursorRect( fav_r, MouseCursor.Link );
        if (GUI.Button( fav_r, favContent, Params.Button ))
        {
          if (favcontains) RemoveFavorite( name );
          else CreateFavorite( name );
        }
      }
#endif



      //  if (selected) GUI.DrawTexture( rect, Button.active.background );

      return GUI.Button( rect, "", Params.Button );
    }

#if !P128
    GUIContent favContent = new GUIContent(){tooltip ="Add to favorites\nCONTROL+CLICK to add to favorites unselected LUTs"};
#endif


    Texture2D TakeScreen(Camera camera, int resWidth, int resHeight)
    {
      var rt = new RenderTexture(resWidth, resHeight, 24);
      var oldT = camera.targetTexture;
      camera.targetTexture = rt;
      var screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false, true);
      camera.Render();
      RenderTexture.active = rt;
      screenShot.ReadPixels( new Rect( 0, 0, resWidth, resHeight ), 0, 0 );
      camera.targetTexture = oldT;
      RenderTexture.active = null;
      DestroyImmediate( rt );
      screenShot.hideFlags = HideFlags.DontSave;
      screenShot.name = "EModules/" + Params.TITLE + "/ScreenShot";
      screenShot.Apply();
      return screenShot;
    }

  }



  public class CachedFloat
  {
    string _key;
    float? _value;
    readonly float _defaultValue;

    public CachedFloat(string key)
    {
      this._key = "EModules/" + Params.TITLE + "/" + key;
      this._value = null;
      this._defaultValue = 0;
    }
    public CachedFloat(string key, float value)
    {
      this._key = "EModules/" + Params.TITLE + "/" + key;
      this._value = null;
      this._defaultValue = value;
    }

    public static implicit operator float(CachedFloat cf) { return cf._value ?? (cf._value = EditorPrefs.GetFloat( cf._key, cf._defaultValue )).Value; }
    public void Set(float value)
    {
      this._value = value;
      EditorPrefs.SetFloat( _key, value );
    }
  }

  public class CachedString
  {
    string _key;
    string _value;
    readonly string _defaultValue;

    public CachedString(string key)
    {
      this._key = "EModules/" + Params.TITLE + "/" + key;
      this._value = null;
      this._defaultValue = "";
    }
    public CachedString(string key, string value)
    {
      this._key = "EModules/" + Params.TITLE + "/" + key;
      this._value = null;
      this._defaultValue = value;
    }

    public static implicit operator string(CachedString cf) { return cf._value ?? (cf._value = EditorPrefs.GetString( cf._key, cf._defaultValue )); }
    public void Set(string value)
    {
      this._value = value;
      EditorPrefs.SetString( _key, value );
    }
  }
}

#endif
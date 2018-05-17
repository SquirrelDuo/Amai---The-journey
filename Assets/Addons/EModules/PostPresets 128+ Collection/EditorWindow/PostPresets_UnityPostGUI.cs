#if UNITY_EDITOR
#define P128

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

#if P128
namespace EModules.PostPresets128
#else
namespace EModules.PostPresets1000
#endif
{
  class PostPresets_UnityPostGUI : ISupportedPostComponent
  {
    string ISupportedPostComponent.GetHashString() { return EditorJsonUtility.ToJson( profile ); }
    void ISupportedPostComponent.CREATE_UNDO(string undoName) { Undo.RecordObject( profile, undoName ); }
    void ISupportedPostComponent.SET_DIRTY() { EditorUtility.SetDirty( profile ); }
    bool ISupportedPostComponent.IsValid { get { return profile; } }
    Type ISupportedPostComponent.MonoComponentType { get { return Params.PostProcessingBehaviourType; } }

    MonoBehaviour _mMonoComponent;
    MonoBehaviour ISupportedPostComponent.MonoComponent
    {
      get {
        if (!_mMonoComponent) _mMonoComponent = Camera.main.GetComponent( Params.PostProcessingBehaviourType ) as MonoBehaviour;
        return _mMonoComponent;
      }
      set { _mMonoComponent = value; }
    }


    public bool AntiAliasEnable
    {
      get { return GetPostprocessingModel( "antialiasing", profile ).enabled; }
      set { GetPostprocessingModel( "antialiasing", profile ).enabled = value; }
    }

    bool ISupportedPostComponent.LutEnable
    {
      get { return GetPostprocessingModel( "userLut", profile ).enabled; }
      set { GetPostprocessingModel( "userLut", profile ).enabled = value; }
    }

    Texture2D ISupportedPostComponent.LutTexture
    {
      get { return GetFieldValue( "lut", GetPostprocessingModel( "userLut", profile ).settings ) as Texture2D; }
      set {
        var s=  GetPostprocessingModel( "userLut", profile ).settings;
        SetFieldValue( "lut", s, value );
        GetPostprocessingModel( "userLut", profile ).settings = s;
      }
    }

    float ISupportedPostComponent.LutAmount
    {
      get { return (float)GetFieldValue( "contribution", GetPostprocessingModel( "userLut", profile ).settings ); }
      set {
        var s=  GetPostprocessingModel( "userLut", profile ).settings;
        SetFieldValue( "contribution", s, value );
        GetPostprocessingModel( "userLut", profile ).settings = s;
      }
    }


    MonoBehaviour PostProcessingBehaviourOjbect { get { return _mMonoComponent; } }
    ScriptableObject profile;
    /* PostProcessingBehaviour PostProcessingBehaviourOjbect { get { return _mMonoComponent as PostProcessingBehaviour; } }
     PostProcessingProfile profile;*/

    Dictionary<ScriptableObject, Editor> p_to_e = new Dictionary<ScriptableObject, Editor>();
    static RenderTexture m_historyTexture;

    const int LAST_COUNT =5;
    static Type ModelType;


    ////////////////////////////
    //! POSTPROCESSING COMPONENT GUI *** //

    void ISupportedPostComponent.LeftSideGUI(EditorWindow window, float width)
    {



      //profile = PostProcessingBehaviourOjbect.profile;
      profile = GetFieldValue( "profile", PostProcessingBehaviourOjbect ) as ScriptableObject;

      var changedProfile = EditorGUILayout.ObjectField( profile, Params.PostProcessingProfileType, false ) as ScriptableObject;
      if (changedProfile != profile)
      {
        Undo.RecordObject( PostProcessingBehaviourOjbect, "Change PostProcessing Profile" );
        profile = SetFieldValue( "profile", PostProcessingBehaviourOjbect, changedProfile ) as ScriptableObject;
        SetLast( changedProfile );
        EditorUtility.SetDirty( PostProcessingBehaviourOjbect );
        EditorUtility.SetDirty( Camera.main.gameObject );
      }

      GUILayout.BeginHorizontal( GUILayout.Width( width ) );
      for (int i = 0 ; i < LastList.Count ; i++)
      {
        if (!LastList[i]) continue;
        var al = Params.Button.alignment;
        Params.Button.alignment = TextAnchor.MiddleLeft;
        var result = GUILayout.Button( Params.CONTENT(LastList[i].name, "Set " + LastList[i].name) , Params.Button, GUILayout.Width( width / LastList.Count ), GUILayout.Height( 14 ) );
        Params.Button.alignment = al;
        if (result)
        {
          Undo.RecordObject( PostProcessingBehaviourOjbect, "Change PostProcessing Profile" );
          SetFieldValue( "profile", PostProcessingBehaviourOjbect, LastList[i] );
          EditorUtility.SetDirty( PostProcessingBehaviourOjbect );
          EditorUtility.SetDirty( Camera.main.gameObject );
        }
        EditorGUIUtility.AddCursorRect( GUILayoutUtility.GetLastRect(), MouseCursor.Link );

      }
      GUILayout.EndHorizontal();

      //GUILayout.Space( 10 );
      GUILayout.BeginHorizontal( GUILayout.Width( width ) );
      if (GUILayout.Button( Params.CONTENT( "Default Profile", "Set Default Profile" ) ))
      {
        var defaultProfile =  AssetDatabase.LoadAssetAtPath( Params.EditorResourcesPath + "/CameraProfile.asset", Params.PostProcessingProfileType ) as ScriptableObject;
        if (!defaultProfile) defaultProfile = CreateProfile( Params.EditorResourcesPath + "/CameraProfile.asset" );
        Undo.RecordObject( PostProcessingBehaviourOjbect, "Change PostProcessing Profile" );
        SetFieldValue( "profile", PostProcessingBehaviourOjbect, defaultProfile );
        SetLast( defaultProfile );
        EditorUtility.SetDirty( PostProcessingBehaviourOjbect );
        EditorUtility.SetDirty( Camera.main.gameObject );
      }
      GUI.enabled = profile;
      if (GUILayout.Button( Params.CONTENT( "New Copy", "Create and set a copy of current profile" ) ))
      {
        var json = EditorJsonUtility.ToJson(profile);
        var newProfile = CreateProfile(AssetDatabase.GenerateUniqueAssetPath( "Assets/NewCameraProfile.asset" ) );
        EditorJsonUtility.FromJsonOverwrite( json, newProfile );
        EditorUtility.SetDirty( newProfile );
        Undo.RecordObject( PostProcessingBehaviourOjbect, "Change PostProcessing Profile" );
        SetFieldValue( "profile", PostProcessingBehaviourOjbect, newProfile );
        SetLast( newProfile );
        EditorUtility.SetDirty( PostProcessingBehaviourOjbect );
        EditorUtility.SetDirty( Camera.main.gameObject );
      }
      GUI.enabled = true;


      GUILayout.EndHorizontal();

      if (!profile)
      {
        return;
      }

      ModelType = GetField( "fog", profile ).FieldType.BaseType;

      if (!p_to_e.ContainsKey( profile ))
        p_to_e.Add( profile, Editor.CreateEditor( profile ) );
      var e = p_to_e[profile];
      if (!e)
      {
        GUILayout.Label( "Internal Plugin Error", Params.Label );
        return;
      }


      Params.AutoRefresh.Set( EditorGUILayout.ToggleLeft( "Automatic refresh when changing", Params.AutoRefresh == 1 ) ? 1 : 0 );


      GUILayout.Space( 10 );

      Params.scroll.x = Params.scrollX;
      Params.scroll.y = Params.scrollY;
      Params.scroll = GUILayout.BeginScrollView( Params.scroll, alwaysShowVertical: true, alwaysShowHorizontal: false );
      Params.scrollX.Set( Params.scroll.x );
      Params.scrollY.Set( Params.scroll.y );
      e.OnInspectorGUI();
      GUILayout.EndScrollView();

    } //! POSTPROCESSING COMPONENT GUI






    ////////////////////////////
    //! TOP FAST BUTTIONS *** //
    void ISupportedPostComponent.TopFastButtonsGUI(EditorWindow window, float leftWidth)
    {
      GUILayout.BeginHorizontal();
      DrawPostProcessingModelButton( "antialiasing", GetPostprocessingModel( "antialiasing", profile ), window, leftWidth );
      DrawPostProcessingModelButton( "ambient\nOcclusion", GetPostprocessingModel( "ambientOcclusion", profile ), window, leftWidth );
      DrawPostProcessingModelButton( "bloom", GetPostprocessingModel( "bloom", profile ), window, leftWidth );
      DrawPostProcessingModelButton( "screenSpace\nReflection", GetPostprocessingModel( "screenSpaceReflection", profile ), window, leftWidth );
      DrawPostProcessingModelButton( "depthOfField", GetPostprocessingModel( "depthOfField", profile ), window, leftWidth );
      DrawPostProcessingModelButton( "fog", GetPostprocessingModel( "fog", profile ), window, leftWidth );
      DrawPostProcessingModelButton( "color\nGrading", GetPostprocessingModel( "colorGrading", profile ), window, leftWidth );


      GUILayout.EndHorizontal();

      //new GUIContent("blend"),

      GUILayout.Space( 20 );
    }

    ScriptableObject CreateProfile(string path)
    {
      var profile = ScriptableObject.CreateInstance(Params.PostProcessingProfileType);
      profile.name = Path.GetFileName( path );
      GetPostprocessingModel( "fog", profile ).enabled = true;
      AssetDatabase.CreateAsset( profile, path );
      return profile;
    }


    void DrawPostProcessingModelButton(string name, PostProcessingModelFake model, EditorWindow window, float leftWidth)
    {
      var rect = EditorGUILayout.GetControlRect( GUILayout.Width( Math.Min( (window.position.width - leftWidth - 10 - 40) / 7, 100 ) ), GUILayout.Height( 40 ) );
      if (model.enabled) EditorGUI.DrawRect( rect, new Color( 0.33f, 0.6f, 0.8f, 0.4f ) );
      EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
      if (GUI.Button( rect, Params.CONTENT( name, "enable/disable " + name.Replace( '\n', ' ' ) ), Params.Button ))
      {
        Undo.RecordObject( profile, "enable/disable " + name );
        model.enabled = !model.enabled;
        EditorUtility.SetDirty( profile );
        window.Repaint();
      }
    } //! TOP FAST BUTTIONS 





    class PostProcessingModelFake
    {
      public PostProcessingModelFake(object target)
      {
        this.target = target;
      }
      object target;
      PropertyInfo _mEnabledProp = null;
      PropertyInfo enabledProp { get { return _mEnabledProp ?? (_mEnabledProp = ModelType.GetProperty( "enabled", (BindingFlags)int.MaxValue )); } }

      public bool enabled
      {
        get { return (bool)enabledProp.GetValue( target, null ); }
        set { enabledProp.SetValue( target, value, null ); }
      }
      public object settings
      {
        get { return GetFieldValue( "m_Settings", target ); }
        set { SetFieldValue( "m_Settings", target, value ); }
      }

    }

    Dictionary<ScriptableObject, Dictionary<string, PostProcessingModelFake>> _mPostModelsFields = new Dictionary<ScriptableObject,Dictionary<string, PostProcessingModelFake>>();
    PostProcessingModelFake GetPostprocessingModel(string name, ScriptableObject profile)
    {
      if (!_mPostModelsFields.ContainsKey( profile )) _mPostModelsFields.Add( profile, new Dictionary<string, PostProcessingModelFake>() );
      if (!_mPostModelsFields[profile].ContainsKey( name ))
      {
        var target = Params.PostProcessingProfileType.GetField( name, (BindingFlags)int.MaxValue ).GetValue( profile );
        _mPostModelsFields[profile].Add( name, new PostProcessingModelFake( target ) );
      }
      return _mPostModelsFields[profile][name];
    }

    static Dictionary<Type, Dictionary<string, FieldInfo>> _mGetFieldValue = new Dictionary<Type,Dictionary<string, FieldInfo>>();
    static FieldInfo GetField(string name, object o)
    {
      var type = o.GetType();
      if (!_mGetFieldValue.ContainsKey( type )) _mGetFieldValue.Add( type, new Dictionary<string, FieldInfo>() );
      if (!_mGetFieldValue[type].ContainsKey( name ))
        _mGetFieldValue[type].Add( name, type.GetField( name, (BindingFlags)int.MaxValue ) );
      return _mGetFieldValue[type][name];
    }
    static object GetFieldValue(string name, object o)
    {
      return GetField( name, o ).GetValue( o );
    }
    static object SetFieldValue(string name, object o, object value)
    {
      GetField( name, o ).SetValue( o, value );
      return value;
    }



    /////////////////
    //! LAST PROFILES *** //

    List<ScriptableObject> _mLastList;
    List<ScriptableObject> LastList
    {
      get {
        if (_mLastList == null)
        {
          _mLastList = new List<ScriptableObject>();
          for (int i = 0 ; i < LAST_COUNT ; i++)
          {
            var guid = EditorPrefs.GetString( "EModules/"+ Params.TITLE +"/LastProfiles" + i,"" );
            if (string.IsNullOrEmpty( guid )) continue;
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty( path )) continue;
            var pr = AssetDatabase.LoadAssetAtPath(path, Params.PostProcessingProfileType) as ScriptableObject;
            if (pr) _mLastList.Add( pr );
          }
        }
        return _mLastList;
      }
      set {
        while (value.Count > LAST_COUNT) value.RemoveAt( LAST_COUNT );
        for (int i = 0 ; i < value.Count ; i++)
        {
          if (!value[i]) continue;
          var path = AssetDatabase.GetAssetPath( value[i] );
          if (string.IsNullOrEmpty( path )) continue;
          var guid = AssetDatabase.AssetPathToGUID(path);
          if (string.IsNullOrEmpty( guid )) continue;
          EditorPrefs.SetString( "EModules/" + Params.TITLE + "/LastProfiles" + i, guid );
        }
        _mLastList = value;
      }
    }



    void SetLast(ScriptableObject newProfile)
    {
      if (!newProfile) return;
      var list = LastList;
      list.Remove( newProfile );
      if (list.Count == 0) list.Add( newProfile ); else list.Insert( 0, newProfile );
      LastList = list;
    }

    //! LAST PROFILES *** //
    /////////////////







    ////////////////////////////
    //! PRESETS GRID GUI *** //

    bool need_postDraw ;
    void ISupportedPostComponent.CameraPredrawAction()
    {
      need_postDraw = false;

      if (GetPostprocessingModel( "antialiasing", profile ).enabled)
      {
        var method = GetFieldValue( "method" , GetPostprocessingModel( "antialiasing", profile ).settings );

        if (method.ToString() == "Taa")
        {
          var fieldTaa =  GetField( "m_Taa", PostProcessingBehaviourOjbect );
          if (fieldTaa != null && GetFieldValue( "m_Taa", PostProcessingBehaviourOjbect ) != null)
          {
            var taa = GetFieldValue("m_Taa",  PostProcessingBehaviourOjbect );

            var fieldm_ResetHistory =  GetField( "m_ResetHistory", taa );
            var fieldm_HistoryTexture =  GetField( "m_HistoryTexture", taa );


            if (fieldm_ResetHistory != null && fieldm_HistoryTexture != null)
            {
              var  m_HistoryTexture = GetFieldValue( "m_HistoryTexture", taa )as RenderTexture;

              if (m_HistoryTexture)
              {
                need_postDraw = true;
                m_historyTexture = m_HistoryTexture;
                SetFieldValue( "m_HistoryTexture", taa, null );
              }
            }
          }
        }
      }
    }

    void ISupportedPostComponent.CameraPostDrawAction()
    {
      if (need_postDraw)
      {
        var taa = GetFieldValue("m_Taa",  PostProcessingBehaviourOjbect );
        SetFieldValue( "m_ResetHistory", taa, false );
        SetFieldValue( "m_HistoryTexture", taa, m_historyTexture );
      }
    }//! PRESETS GRID GUI


    /* void ISupportedPostComponent.CameraPredrawAction()
     {
       need_postDraw = false;

       if (profile.antialiasing.enabled && profile.antialiasing.settings.method == AntialiasingModel.Method.Taa)
       {
         var m_Taa = typeof( PostProcessingBehaviour ).GetField( "m_Taa", (BindingFlags)int.MaxValue );
         var  m_ResetHistory = typeof( TaaComponent ).GetField( "m_ResetHistory", (BindingFlags)int.MaxValue );
         var  m_HistoryTexture = typeof( TaaComponent ).GetField( "m_HistoryTexture", (BindingFlags)int.MaxValue );
         if (m_Taa != null && m_ResetHistory != null && m_HistoryTexture != null)
         {
           var taa = m_Taa.GetValue( PostProcessingBehaviourOjbect ) as TaaComponent;
           if (taa != null)
           {
             var his = m_HistoryTexture.GetValue( taa ) as RenderTexture;
             if (his)
             {
               need_postDraw = true;
               m_historyTexture = his;
               m_HistoryTexture.SetValue( taa, null );
             }
           }
         }
       }
     }

     void ISupportedPostComponent.CameraPostDrawAction()
     {
       if (need_postDraw)
       {
         var m_Taa = typeof( PostProcessingBehaviour ).GetField( "m_Taa", (BindingFlags)int.MaxValue );
         var  m_ResetHistory = typeof( TaaComponent ).GetField( "m_ResetHistory", (BindingFlags)int.MaxValue );
         var  m_HistoryTexture = typeof( TaaComponent ).GetField( "m_HistoryTexture", (BindingFlags)int.MaxValue );
         var taa = m_Taa.GetValue( PostProcessingBehaviourOjbect );
         m_ResetHistory.SetValue( taa, false );
         m_HistoryTexture.SetValue( taa, m_historyTexture );
       }
     }//! PRESETS GRID GUI*/







    // ////////////////////
    //! FieldsHelper *** //



    //! FieldsHelper *** //
    // ////////////////////
  }
}
#endif

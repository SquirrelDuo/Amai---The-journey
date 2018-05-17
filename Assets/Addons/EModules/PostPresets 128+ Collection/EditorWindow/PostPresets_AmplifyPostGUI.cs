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
  class PostPresets_AmplifyPostGUI : ISupportedPostComponent
  {
    string ISupportedPostComponent.GetHashString() { return EditorJsonUtility.ToJson( ((ISupportedPostComponent)this).MonoComponent ); }
    void ISupportedPostComponent.CREATE_UNDO(string undoName) { Undo.RecordObject( ((ISupportedPostComponent)this).MonoComponent, undoName ); }
    void ISupportedPostComponent.SET_DIRTY() { EditorUtility.SetDirty( ((ISupportedPostComponent)this).MonoComponent ); }
    Type ISupportedPostComponent.MonoComponentType { get { return Params.AmplifyEffectType; } }

    // SKIP //
    bool ISupportedPostComponent.IsValid { get { return true; } }
    bool ISupportedPostComponent.AntiAliasEnable { get { return true; } set { } }
    bool ISupportedPostComponent.LutEnable { get { return true; } set { } }
    void ISupportedPostComponent.CameraPredrawAction() { }
    void ISupportedPostComponent.CameraPostDrawAction() { }
    // SKIP //

    Texture2D ISupportedPostComponent.LutTexture
    {
      get { return LutTexture.GetValue( ((ISupportedPostComponent)this).MonoComponent ) as Texture2D; }
      set { LutTexture.SetValue( ((ISupportedPostComponent)this).MonoComponent, value ); }
    }

    float ISupportedPostComponent.LutAmount
    {
      get { return 1 - (float)BlendAmount.GetValue( ((ISupportedPostComponent)this).MonoComponent ); }
      set { BlendAmount.SetValue( ((ISupportedPostComponent)this).MonoComponent, Mathf.Clamp01( 1 - value ) ); }
    }

    MonoBehaviour _mMonoComponent;
    MonoBehaviour ISupportedPostComponent.MonoComponent
    {
      get {
        if (!_mMonoComponent) _mMonoComponent = Camera.main.GetComponent( Params.AmplifyBaseType ) as MonoBehaviour;
        return _mMonoComponent;
      }
      set { _mMonoComponent = value; }
    }


    Dictionary<MonoBehaviour, Editor> p_to_e = new Dictionary<MonoBehaviour, Editor>();

    void ISupportedPostComponent.LeftSideGUI(EditorWindow window, float width)
    {
      var c =  ((ISupportedPostComponent)this).MonoComponent;

      if (!p_to_e.ContainsKey( c ))
        p_to_e.Add( c, Editor.CreateEditor( c ) );
      var e = p_to_e[c];
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

    }

    void ISupportedPostComponent.TopFastButtonsGUI(EditorWindow window, float width)
    {
      GUILayout.Label( "The fast component button is not available for AmplifyColor", GUILayout.Height( 40 ) );
      GUILayout.Space( 20 );
    }


    // ////////////////////
    //! FieldsHelper *** //

    FieldInfo _mBlendAmount;
    FieldInfo BlendAmount { get { return _mBlendAmount ?? (_mBlendAmount = Params.AmplifyBaseType.GetField( "BlendAmount", (BindingFlags)int.MaxValue )); } }

    FieldInfo _mLutTexture;
    FieldInfo LutTexture { get { return _mLutTexture ?? (_mLutTexture = Params.AmplifyBaseType.GetField( "LutTexture", (BindingFlags)int.MaxValue )); } }

    //! FieldsHelper *** //
    // ////////////////////
  }
}
#endif

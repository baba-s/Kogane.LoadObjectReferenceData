using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Kogane
{
    /// <summary>
    /// 読み込んだオブジェクトの参照カウンタを管理するクラス
    /// </summary>
    [Serializable]
    public sealed class LoadObjectReferenceData<T> : IReadOnlyLoadObjectReferenceData where T : Object
    {
        //================================================================================
        // 変数(SerializeField)
        //================================================================================
        [SerializeField][UsedImplicitly] private string m_path;      // 読み込んだオブジェクトのパス
        [SerializeField]                 private int    m_count = 1; // オブジェクトを参照しているハンドルの数
        [SerializeField]                 private bool   m_isDone;    // すでに読み込みが完了している場合 true

        //================================================================================
        // 変数(readonly)
        //================================================================================
        private Action<T> m_onUnload; // アンロードする時に呼び出されるコールバック

        //================================================================================
        // 変数
        //================================================================================
        private T      m_object;      // 読み込んだオブジェクト
        private Action m_onCompleted; // 読み込みが完了した時に呼び出されるコールバック

        //================================================================================
        // プロパティ
        //================================================================================
        string IReadOnlyLoadObjectReferenceData.Path   => m_path;
        int IReadOnlyLoadObjectReferenceData.   Count  => m_count;
        bool IReadOnlyLoadObjectReferenceData.  IsDone => m_isDone;

        //================================================================================
        // 関数
        //================================================================================
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoadObjectReferenceData( string path, Action<T> onUnload )
        {
            m_path     = path;
            m_onUnload = onUnload;
        }

        /// <summary>
        /// オブジェクトを読み込んだ時に呼び出します
        /// </summary>
        public void OnLoad( T obj )
        {
            m_object = obj;
            m_isDone = true;

            m_onCompleted?.Invoke();
            m_onCompleted = null;
        }

        /// <summary>
        /// 読み込んだオブジェクトを取得する時に呼び出します
        /// </summary>
        public async UniTask<T> GetAsync()
        {
            // オブジェクトを読み込んでいる途中の場合
            if ( !m_isDone )
            {
                // 読み込みが完了するまで待機します
                var tcs = new UniTaskCompletionSource();
                m_onCompleted += () => tcs.TrySetResult();
                await tcs.Task;
            }

            m_count++;
            return m_object;
        }

        /// <summary>
        /// 読み込んだオブジェクトをアンロードする時に呼び出します
        /// </summary>
        public bool OnUnload()
        {
            try
            {
                m_count--;
                if ( 0 < m_count ) return false;
                m_onUnload?.Invoke( m_object );
                m_object = null;
                return true;
            }
            // 念のため NullReferenceException が発生したら握りつぶす
            // （握りつぶしてもおそらく問題はないため）
            catch ( NullReferenceException )
            {
                return false;
            }
        }

        /// <summary>
        /// JSON 形式の文字列に変換して返します
        /// </summary>
        public override string ToString()
        {
            return JsonUtility.ToJson( this, true );
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kogane
{
    /// <summary>
    /// 読み込んだオブジェクトの参照カウンタのリストを管理するクラス
    /// </summary>
    public sealed class LoadObjectReferenceList<T> : IEnumerable<IReadOnlyLoadObjectReferenceData> where T : Object
    {
        //================================================================================
        // 変数
        //================================================================================
        private readonly Dictionary<string, LoadObjectReferenceData<T>> m_dictionary = new();

        //================================================================================
        // 関数
        //================================================================================
        /// <summary>
        /// 指定されたアセットのパスに紐づく参照カウンタを追加します
        /// </summary>
        public void Add
        (
            string                     assetPath,
            LoadObjectReferenceData<T> referenceData
        )
        {
            m_dictionary[ assetPath ] = referenceData;
        }

        /// <summary>
        /// 指定されたアセットのパスに紐づく参照カウンタを返します
        /// </summary>
        public LoadObjectReferenceData<T> Get( string assetPath )
        {
            return m_dictionary[ assetPath ];
        }

        /// <summary>
        /// 指定されたアセットのパスに紐づく参照カウンタを返します
        /// </summary>
        public bool TryGet
        (
            string                         assetPath,
            out LoadObjectReferenceData<T> referenceData
        )
        {
            return m_dictionary.TryGetValue( assetPath, out referenceData );
        }

        /// <summary>
        /// 指定されたアセットのパスに紐づく参照カウンタを削除します
        /// </summary>
        public void Remove( string assetPath )
        {
            m_dictionary.Remove( assetPath );
        }

        public IEnumerator<IReadOnlyLoadObjectReferenceData> GetEnumerator()
        {
            return m_dictionary.Values.Cast<IReadOnlyLoadObjectReferenceData>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
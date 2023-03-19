using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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
        // プロパティ
        //================================================================================
        public int Count => m_dictionary.Count;

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
        [CanBeNull]
        public LoadObjectReferenceData<T> Get( string assetPath )
        {
            // 高速でエージングテストしている時に以下の例外が出ることがあったため
            // 念の為、キーの存在をチェックしています
            // KeyNotFoundException: The given key 'XXXX' was not present in the dictionary.
            // System.Collections.Generic.Dictionary`2[TKey,TValue].get_Item (TKey key) (at <f64c40317fe34259bdfbc41d7b5cbc96>:0)
            // Kogane.LoadObjectReferenceList`1[T].Get (System.String assetPath) (at UnityProject/Packages/Kogane.LoadObjectReferenceData/Runtime/LoadObjectReferenceList.cs:38)
            return m_dictionary.ContainsKey( assetPath )
                    ? m_dictionary[ assetPath ]
                    : null
                ;
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
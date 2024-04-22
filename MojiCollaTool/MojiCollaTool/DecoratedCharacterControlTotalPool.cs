using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MojiCollaTool
{
    public class DecoratedCharacterControlTotalPool
    {
        /// <summary>
        /// 文字オブジェクトの再利用プールとなる辞書
        /// </summary>
        private Dictionary<char, DecoratedCharacterControlPool> pool = new Dictionary<char, DecoratedCharacterControlPool>();

        /// <summary>
        /// プールをクリアする
        /// </summary>
        public void Clear()
        {
            pool.Clear();
        }

        /// <summary>
        /// 文字オブジェクトを返す
        /// </summary>
        /// <param name="character"></param>
        public DecoratedCharacterControl GetDecoratedCharacterControl(char character, MojiData mojiData)
        {
            if (pool.ContainsKey(character))
            {
                //  その文字がプールに存在する場合、プールから返す
                return pool[character].GetDecoratedCharacterControl(mojiData);
            }
            else
            {
                //  その文字が存在しない場合、プールを新規作成して、そこから返す
                var pool = new DecoratedCharacterControlPool(character);

                this.pool[character] = pool;

                return pool.GetDecoratedCharacterControl(mojiData);
            }
        }

        /// <summary>
        /// 文字オブジェクトプールの使用数カウンターをリセットする
        /// </summary>
        public void ResetUsedCounter()
        {
            foreach (var keyValuePair in pool)
            {
                keyValuePair.Value.ResetUsedCounter();
            }
        }
    }
}

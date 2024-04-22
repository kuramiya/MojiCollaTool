using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojiCollaTool
{
    public class DecoratedCharacterControlPool
    {
        /// <summary>
        /// 文字
        /// </summary>
        public char Character { get; set; }

        /// <summary>
        /// 要不要フラグ
        /// </summary>
        public bool IsNeeded { get; set; } = false;

        /// <summary>
        /// 使用数カウンター
        /// </summary>
        public int UsedCounter { get; set; } = 0;

        /// <summary>
        /// 文字オブジェクトのリスト
        /// </summary>
        public List<DecoratedCharacterControl> Pool { get; set; } = new List<DecoratedCharacterControl>();

        public DecoratedCharacterControlPool(char character)
        {
            Character = character;
        }

        public DecoratedCharacterControlPool(char character, int requiredCount, MojiData moijData) 
        {
            Character = character;
            UpdatePool(requiredCount, moijData);
        }

        /// <summary>
        /// 使用数カウンターをリセットする
        /// </summary>
        public void ResetUsedCounter()
        {
            UsedCounter = 0;
        }

        /// <summary>
        /// プールの中身を更新する
        /// </summary>
        /// <param name="requiredCount"></param>
        /// <param name="moijData"></param>
        public void UpdatePool(int requiredCount, MojiData moijData)
        {
            IsNeeded = true;

            UsedCounter = 0;

            //  不足数
            var lackCount = requiredCount - Pool.Count;

            if(lackCount > 0)
            {
                //  足りない場合は追加する
                while(requiredCount != Pool.Count)
                {
                    Pool.Add(new DecoratedCharacterControl(Character, moijData));
                }
            }
            else if(lackCount < 0)
            {
                //  余っている場合は削除する
                while (requiredCount != Pool.Count)
                {
                    Pool.Remove(Pool.Last());
                }
            }
        }

        /// <summary>
        /// プール内から文字オブジェクトを返す
        /// </summary>
        /// <returns></returns>
        public DecoratedCharacterControl GetDecoratedCharacterControl(MojiData mojiData)
        {
            if (UsedCounter >= Pool.Count)
            {
                //  プール内に存在しない場合、新規生成する
                Pool.Add(new DecoratedCharacterControl(Character, mojiData));
            }

            var decoratedCharacterControl = Pool[UsedCounter];

            ++UsedCounter;

            return decoratedCharacterControl;
        }
    }
}

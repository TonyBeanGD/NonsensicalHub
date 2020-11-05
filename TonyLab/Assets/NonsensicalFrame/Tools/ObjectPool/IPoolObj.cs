namespace NonsensicalKit
{
    public interface IPoolObj
    {
        /// <summary>
        /// 获取使用情况
        /// </summary>
        /// <returns>返回这个对象是使用状态还是休眠状态，使用中时返回true</returns>
        bool CheckUseState();
        /// <summary>
        /// 对象被取出时调用
        /// </summary>
        void OnSpawn();
        /// <summary>
        /// 对象被放回时调用
        /// </summary>
        void OnUnSpawn();
    }
}


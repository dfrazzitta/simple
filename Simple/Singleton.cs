using System.Reflection;

namespace Simple
{
    public sealed class Singleton
    {
        private static Singleton instance = null;
        private static readonly object padlock = new object();
        
        Singleton()
        {
 
    }

        public void KissAss()
        {
            
            
        }


        public int GetInfo()
        {
            return 0;
        }
  
        public static Singleton Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Singleton();
                    }
                    return instance;
                }
            }
        }
    }
}

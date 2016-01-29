/*==================================
             ########   
            ##########           
                                              
             ########             
            ##########            
          ##############         
         #######  #######        
        ######      ######       
        #####        #####       
        ####          ####       
        ####   ####   ####       
        #####  ####  #####       
         ################        
          ##############                                                 
==================================*/

namespace Business.Cache
{
    public interface ICache<K>
    {
        void Set(K key, CacheValue value, System.TimeSpan? outTime = null);

        CacheValue Get(K key);

        void Remove(K key);

        bool ContainsKey(K key);
    }

    public interface ICache : ICache<string> { }
}

/*
License

Some of this may have been acquired from other sources, whose copyright has 
been lost. So no copyright is claimed and it is unreasonable to grant 
permission to use, copy, modify, etc (as in the normal MIT License). 

If any copyright holders identify their material herein, then the
appropriate copyright notice will be added. 

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Mistware.Utils
{
    /// <summary>
    /// Simple wrapper around the System.Runtime.Caching 
    /// </summary>
    /// <typeparam name="T">The type of object being cached</typeparam>
    public sealed class Cache<T>
        where T : class
    {
     
        //Singleton Design Pattern to ensure thread safety
        private Cache() {}
                
        private static readonly Cache<T> instance = new Cache<T>();
        
        /// Singleton accessor
        public static Cache<T> GetCache()
        {
            return instance;
        }

        /// <summary>
        /// Simple accessor for the CacheManager.  Could store this instead of getting from
        /// the factory each time, but its not proving to be a problem yet.
        /// </summary>
        private static ObjectCache _cache = null;
        private ObjectCache cache
        {
            get
            {
                if (_cache == null) _cache = MemoryCache.Default;
                return _cache;
            }
        }

        /// <summary>
        /// returns cached item of type T, that has been stored against the key. 
        /// Function (delegate) f is used to (re)fill the cache in the event that the key cannot 
        /// be found. It returns the item of type T.
        /// </summary>
        /// <param name="cacheKey">Key of the item</param>
        /// <param name="retriever">Delegate to retrieve item when the cached item isn't there</param>
        /// <returns>The cached item if one is present, or the value returned by the delegate if not</returns>
        public T Get(string cacheKey, Func<T> retriever)
        {
            try
            {
                T item = cache[cacheKey] as T;
                if (item == null) item = Add(cacheKey, retriever);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Cache.Get: " + ex.Message);
            }
        }

        /// <summary>
        /// Fill cache - called with key and function (delegate) to get the object to cache
        /// </summary>
        /// <param name="cacheKey">Key of the item</param>
        /// <param name="retriever">Delegate to retrieve item to cache</param>
        public void Set(string cacheKey, Func<T> retriever)
        {
            _ = Add(cacheKey, retriever);
        }

        private T Add(string cacheKey, Func<T> retriever)            
        {
            try
            {
                T item = null;
                lock (AddLock)
                {
                    item = retriever();
                    cache.Add(cacheKey, item, DateTime.Now.AddHours(1));
                }
                return item;
            } 
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error adding cached item {0}. Error is: {1}", cacheKey, ex.Message));
            }
        }

        private Object AddLock = new Object();

        /// <summary>
        /// Checks if an object is stored in the cache with that key
        /// </summary>
        /// <param name="cacheKey">Key to be checked</param>
        /// <returns>true if key exists, othewise false</returns>
        public bool Exists(string cacheKey)
        {
            return (cache[cacheKey] != null);
        }

        /// <summary>
        /// Deletes object from cache with that key
        /// </summary>
        /// <param name="cacheKey">Key of the item to be deleted</param>
        public void Remove(string cacheKey)
        {
             if (cache[cacheKey] != null) cache.Remove(cacheKey); 
        }

        /// <summary>
        /// Returns list of keys in the cache
        /// </summary>
        /// <returns>a list of strings</returns>
        public List<string> ListKeys()
        {
            List<string> l = new List<string>();

            foreach (KeyValuePair<string,object> kv in cache) 
            {
                l.Add(kv.Key.ToString());                    
            }
            return l;
        } 

    }
}

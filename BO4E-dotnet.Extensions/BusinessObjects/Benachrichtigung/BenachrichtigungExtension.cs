using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json.Linq;
using BO4E.BO;

namespace BO4E.Extensions.BusinessObjects.Benachrichtigung
{
    /// <summary>Additional methods for <see cref="Benachrichtigung"/>.</summary>
    public static partial class BenachrichtigungExtension
    {
        /// <summary>
        /// checks if <see cref="BO4E.BO.Benachrichtigung.infos"/> contains a key value pair
        /// </summary>
        /// <param name="b"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Has(this BO4E.BO.Benachrichtigung b, string key, string value)
        {
            return Has(b, new BO4E.COM.GenericStringStringInfo()
            {
                keyColumn = key,
                value = value
            });
        }

        /// <summary>
        /// checks if <paramref name="b"/> contains a key value pair specified in <paramref name="gssi"/> in <see cref="BO4E.BO.Benachrichtigung.infos"/>
        /// </summary>
        /// <param name="b"></param>
        /// <param name="gssi"></param>
        /// <returns></returns>
        public static bool Has(this BO4E.BO.Benachrichtigung b, BO4E.COM.GenericStringStringInfo gssi)
        {
            if (b.infos == null || b.infos.Count == 0)
            {
                return false;
            }
            // ToDo f�r Hamid: Bitte pr�fen, warum Contains false zur�ckliefert.
            return (b.infos.Where(m => m.keyColumn == gssi.keyColumn && m.value == gssi.value).Count() > 0);
        }

        /// <summary>
        /// check if a specific info exists
        /// </summary>
        /// <param name="b">Benachrichtigung</param>
        /// <param name="key">key to be checked</param>
        /// <returns>true if key is in <see cref="BO4E.BO.Benachrichtigung.infos"/></returns>
        public static bool Has(this BO4E.BO.Benachrichtigung b, string key)
        {
            if (b.infos == null || b.infos.Count == 0)
            {
                return false;
            }
            return (b.infos.Where(gssi => gssi.keyColumn == key).Count() > 0);
        }

        /// <summary>
        /// checks if Benachrichtigung <paramref name="b"/> has an entry with key <paramref name="keyName"/> in <see cref="BO4E.BO.Benachrichtigung.infos"/> which fulfills a predicate
        /// </summary>
        /// <typeparam name="T">expected type of the info property</typeparam>
        /// <param name="b">Benachrichtigung object</param>
        /// <param name="keyName">key name of the info property</param>
        /// <param name="predicate"></param>
        /// <param name="passByDefault">defines default behaviour, e.g. if no such key is present</param>
        /// <param name="typeConverter">allows to provide an explicit type converter</param>
        /// <returns>true if there's an info object with given key <paramref name="keyName"/> of type <typeparamref name="T"/> fulfilling <paramref name="predicate"/> or there's no such property but <paramref name="passByDefault"/> is true</returns>
        public static bool Has<T>(this BO4E.BO.Benachrichtigung b, string keyName, Predicate<T> predicate, bool passByDefault = true, TypeConverter typeConverter = null) where T : IComparable
        {
            if (!b.Has(keyName))
            {
                return passByDefault;
            }
            foreach (var info in b.infos.Where(gssi => gssi.keyColumn == keyName))
            {
                try
                {
                    if (typeConverter == null)
                    {
                        typeConverter = TypeDescriptor.GetConverter(typeof(T));
                    }
                    if (typeConverter != null)
                    {
                        T value = (T)typeConverter.ConvertFromString(info.value);
                        return predicate(value);
                    }
                    continue;
                }
                catch (NotSupportedException)
                {
                    continue;
                }
            }
            return false;
        }

        /// <summary>
        /// moves key value pairs from <see cref="BO4E.BO.Benachrichtigung.infos"/> to <see cref="BO4E.BO.BusinessObject.userProperties"/> for more conventient handling.
        /// </summary>
        /// <param name="b">Benachrichtigung</param>
        /// <param name="overwriteExistingKeys">set true to overwrite userProperties with same key</param>
        // ToDo: make method generic MoveInfosTouserProperties<boT>(...)
        public static void MoveInfosToUserProperties(this BO4E.BO.Benachrichtigung b, bool overwriteExistingKeys = false)
        {
            if (b.infos != null && b.infos.Count > 0)
            {
                if (b.userProperties == null)
                {
                    b.userProperties = new Dictionary<string, JToken>();
                }
                foreach (var info in b.infos)
                {
                    if (b.userProperties.ContainsKey(info.keyColumn) && overwriteExistingKeys)
                    {
                        b.userProperties.Remove(info.keyColumn);
                    }
                    b.userProperties.Add(info.keyColumn, info.value); // might throw exception if key exists and !overwriteExistingKeys. That's ok.
                }
                b.infos = null; // set to null after all elements have been moved
            }
        }
    }
}

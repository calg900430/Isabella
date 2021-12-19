namespace Isabella.Web.Hubs
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Servicio Externo al Hub para el manejo de las conexiones.
    /// </summary>
    public class DicctionaryConnectedHub
    {
        /// <summary>
        /// Representa un diccionario con las conexiones a un Hub especifico.
        /// </summary>
        static readonly Dictionary<string, HashSet<string>> _devicesconnecteds = new Dictionary<string, HashSet<string>>();

        /// <summary>
        /// Devuelve la cantidad de usuarios que están conectados al servidor Hub.
        /// </summary>
        public int CantConnected { get => _devicesconnecteds.Count; }

        /// <summary>
        /// Devuelve los Id de la conexión de los dispositivos de un usuario que están conectados 
        /// actualmente, dada una lista de Keys.
        /// </summary>
        public List<string> GetAllDeviceConnected(List<string> Keys)
        {
            //Verifica que la lista de Keys no este vacia.
            if(Keys == null)
            return null;
            //Verifica cuales son los usuarios que se encuentran en el diccionario
            List<string> user_online_of_list = new List<string>();
            foreach (string key in Keys)
            {
               var user_online = _devicesconnecteds.Where(c => c.Key == key).FirstOrDefault();
               if(user_online.Value == null)
               continue;
               //Convierte la colección Hashset en una lista
               var list_of_hashset = user_online.Value.ToList();
               user_online_of_list.AddRange(list_of_hashset);
            }
            return user_online_of_list;
        }
    
        /// <summary>
        /// Agrega un nuevo dispositivo que se conectó al Hub al diccionario.
        /// </summary>
        /// <param name="key_userName"></param>
        /// <param name="connectionID"></param>
        public void AddNewDevice(string key_userName, string connectionID)
        {
            lock(_devicesconnecteds)
            {
                HashSet<string> connections;
                if (!_devicesconnecteds.TryGetValue(key_userName, out connections))
                {
                    connections = new HashSet<string>();
                    _devicesconnecteds.Add(key_userName, connections);
                }
                lock(connections)
                {
                    connections.Add(connectionID);
                }
            }
        }

        /// <summary>
        /// Elimina un dispositivo del diccionario(Cuando se desconecta del Hub)
        /// </summary>
        public void RemoveDevice(string key_userName, string connectionID)
        {
            lock (_devicesconnecteds)
            {
                HashSet<string> connections;
                if (!_devicesconnecteds.TryGetValue(key_userName, out connections))
                {
                    return;
                }
                lock(connections)
                {
                    connections.Remove(connectionID);
                    if (connections.Count == 0)
                    {
                        _devicesconnecteds.Remove(key_userName);
                    }
                }
            }
        }

        /// <summary>
        /// Devuelve todos los dispositivos que están conectados actualmente de un usuario.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllDeviceConnectedOfUser(string key_UserName)
        {
            HashSet<string> connections;
            if (_devicesconnecteds.TryGetValue(key_UserName, out connections))
            {
                return connections;
            }
            return Enumerable.Empty<string>();
        }      

        /// <summary>
        /// Verifica si un usuario determinado está conectado.
        /// </summary>
        /// <param name="key_userName"></param>
        /// <returns></returns>
        public bool VerifyIsUserConnected(string key_userName)
        {
            HashSet<string> connections = null;
            //Verifica si el usuario está conectado.
            var connected = _devicesconnecteds.TryGetValue(key_userName, out connections);
            if(connected)
            return true;
            else
            return false;
        }
    }
}

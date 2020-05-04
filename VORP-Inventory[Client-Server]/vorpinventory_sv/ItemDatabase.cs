﻿using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace vorpinventory_sv
{
    public class ItemDatabase:BaseScript
    {
        //Lista de items con sus labels para que el cliente conozca el label de cada item
        public static dynamic items;
        //Lista de itemclass con el nombre de su dueño para poder hacer todo el tema de añadir y quitar cuando se robe y demas
        public static Dictionary<string,Dictionary<string,ItemClass>> usersInventory = new Dictionary<string, Dictionary<string, ItemClass>>();
        public static Dictionary<string,Items> svItems = new Dictionary<string, Items>();
        public ItemDatabase()
        {
            Exports["ghmattimysql"].execute("SELECT * FROM items",new Action<dynamic>((result) =>
            {
                if (result.Count == 0)
                {
                    Debug.WriteLine("There`re no items in database");
                }
                else
                {
                    items = result;
                    foreach (dynamic item in items)
                    {
                        svItems.Add(item.item.ToString(),new Items(item.item,item.label,int.Parse(item.limit.ToString()),item.can_remove));
                    }
                    Exports["ghmattimysql"].execute("SELECT identifier,inventory,loadout FROM characters", new Action<dynamic>((uinvento) =>
                    {
                        Dictionary<string,ItemClass> userinv = new Dictionary<string, ItemClass>();
                        foreach (var userInventory in uinvento)
                        {
                            string user = userInventory.identifier.ToString();

                            if (userInventory.inventory != null)
                            {
                                userinv.Clear();
                                dynamic thing = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(userInventory.inventory);
                                foreach (dynamic itemname in items)
                                {
                                    if (thing[itemname.item.ToString()] != null)
                                    {
                                        ItemClass item = new ItemClass(int.Parse(thing[itemname.item.ToString()].ToString()),int.Parse(itemname.limit.ToString()),
                                            itemname.label,itemname.item,"item_inventory",true,itemname.can_remove);
                                        userinv.Add(itemname.item.ToString(),item);
                                    }
                                }
                                usersInventory.Add(user,userinv);
                            }
                            else
                            {
                                usersInventory.Add(user,userinv);
                            }
                        }
                    }));
                }
            }));
        }
    }
}
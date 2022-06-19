using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class PlatformUserData
{
    public string id;
    public string name;
    public string surname;
    public string userClass;
    public Dictionary<string, bool> games;

    public string Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public string Surname { get => surname; set => surname = value; }
    public string UserClass { get => userClass; set => userClass = value; }
    public Dictionary<string, bool> Games { get => games; set => games = value; }

    public PlatformUserData(string id, string name, string surname, string userClass, Dictionary<string, bool> games)
    {
        this.id = id;
        this.name = name;
        this.surname = surname;
        this.userClass = userClass;
        this.games = games;
    }

    public override string ToString()
    {
        return $"PlatformUserData : [id - {id}, fullname - {name} {surname}, userClass - {userClass}]";
    }
}


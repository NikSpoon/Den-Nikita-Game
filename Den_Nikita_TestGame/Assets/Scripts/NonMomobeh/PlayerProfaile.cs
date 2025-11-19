
public class PlayerProfaile
{
    public string Name { get; private set; }
    public string Pasword { get; private set; }
    public int ID { get; private set; }
    public int Lewl { get; private set; }

    public void InitNewProfail(string name , string pasword)
    {
        Name = name;
        Pasword = pasword;
        ID = 1;
        Lewl = 1;
    }
}

// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");

if (args.Length == 0)
{
    Console.WriteLine("Supported commands:");
    Console.WriteLine("\tcopy");
    return;
}

if (args[0] == "copy")
{
    File.Copy(args[1], args[2], true);
}

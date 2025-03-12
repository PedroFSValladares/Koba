using Koba.Core;
using DotNetEnv;

Env.Load(".env");
await new KobaCore().Run();
await Task.Delay(Timeout.Infinite);


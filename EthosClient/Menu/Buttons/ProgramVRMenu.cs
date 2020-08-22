using RubyButtonAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EthosClient.Menu.Buttons
{
    public class ProgramVRMenu : QMNestedButton
    {
        public ProgramVRMenu(QMNestedButton parent) : base(parent, 1, 1, "Programs", "Easily control programs on your computer.", Color.red, Color.white, Color.red, Color.white)
        {
            var Programs = Process.GetProcesses();
            int x = 0;
            int y = 0;
            for(var i = 0; i < Programs.Count(); i++)
            {
                if (x == 4)
                {
                    if (y != 2)
                    {
                        new QMSingleButton(this, x, y, Programs[i].ProcessName, delegate
                        {
                            var programs = Process.GetProcessesByName(Programs[i].ProcessName);

                            if (programs.Count() == 0)
                                Process.Start(Programs[i].ProcessName + ".exe");
                            else
                                foreach (var process in programs)
                                    process.Kill();

                        }, $"Opens/Closes {Programs[i].ProcessName}", Color.red, Color.white);
                        y++;
                    }
                }
                else
                {
                    new QMSingleButton(this, x, y, Programs[i].ProcessName, delegate
                    {
                        var programs = Process.GetProcessesByName(Programs[i].ProcessName);

                        if (programs.Count() == 0)
                            Process.Start(Programs[i].ProcessName + ".exe");
                        else
                            foreach (var process in programs)
                                process.Kill();

                    }, $"Opens/Closes {Programs[i].ProcessName}", Color.red, Color.white);
                    x++;
                }
            }
        }
    }
}

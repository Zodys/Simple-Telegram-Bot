using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace telebot.Commands
{
    public interface ICommand
    {
        string Execute(params object[] args);
    }


}
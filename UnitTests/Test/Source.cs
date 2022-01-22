using System.Collections.Generic;
using DependencyInjectionContainer.Api.Attributes;
using DependencyInjectionContainer.Api.Parameters;

namespace UnitTests.Test
{
    interface ICommand
    {
        int TestCommand();
    }

    public class MyCommand1 : ICommand
    {
        public int TestCommand()
        {
            return 1;
        }
    }

    public class MyCommand2 : ICommand
    {
        public int TestCommand()
        {
            return 2;
        }
    }

    public interface IRepository { }

    public class Repository : IRepository { }

    public interface IChat
    {
        string SendMessage();
    }

    public class TelegramChat : IChat
    {
        public IEnumerable<IRepository> Repositories;

        public TelegramChat(IEnumerable<IRepository> repositories)
        {
            Repositories = repositories;
        }

        public string SendMessage()
        {
            return "Telegram";
        }
    }

    public class VkChat : IChat
    {
        public IEnumerable<IRepository> Repositories;

        public VkChat(IEnumerable<IRepository> repositories)
        {
            Repositories = repositories;
        }

        public string SendMessage()
        {
            return "Vk";
        }
    }

    interface IFirstChat
    {
        IChat GetChat();
    }

    public class FirstChat : IFirstChat
    {
        public IChat Chat;

        public FirstChat([DependencyKey(ServiceImplementations.First)] IChat chat)
        {
            Chat = chat;
        }

        public IChat GetChat()
        {
            return Chat;
        }
    }
}

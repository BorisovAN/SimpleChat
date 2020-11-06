using SimpleChat.Proto;
using SimpleChat.Server.Storage.DBO;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace SimpleChat.Server.Storage
{
    /*\
     * <DataRoot>\Users\<UserId>.bin \UserInfo + DialogsIds
     * <DataRoot>\Dialogs\<DialogId>.bin\User1Id User2Id
     * <DataRoot>\Messages\<DialogId>\<MessageId>.txt
     */
    //см. интерфейс IStorage

    public class FileStorage : IStorage
    {
        private const string DataRoot = @"D:\ChatData\";
        private static readonly string DialogsFolder = Path.Combine(DataRoot, "Dialogs");
        private static readonly string UsersFolder = Path.Combine(DataRoot, "Users");
        private static readonly string MessagesFolder = Path.Combine(DataRoot, "Messages");

        private User GetUserById(Guid id)
        {
            var directory = new DirectoryInfo(UsersFolder);
            var formatter = new BinaryFormatter();
            User user;
            foreach (var fileInfo in directory.GetFiles())
            {

                using (var stream = fileInfo.Open(FileMode.Open))
                {
                    user = formatter.Deserialize(stream) as User;
                }

                if (user.Id == id)
                {
                    return user;
                }
            }
            throw new InvalidOperationException($"User with id {id} not found");
        }

        public void Dispose() { }

        static FileStorage()
        {
            Directory.CreateDirectory(DialogsFolder);
            Directory.CreateDirectory(UsersFolder);
            Directory.CreateDirectory(MessagesFolder);
        }

        public Guid Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return Guid.Empty;

            var directory = new DirectoryInfo(UsersFolder);
            var formatter = new BinaryFormatter();
            foreach (var fileInfo in directory.GetFiles())
            {
                User user;
                using (var stream = fileInfo.Open(FileMode.Open))
                {
                    user = formatter.Deserialize(stream) as User;
                }

                if (user.UserName == username && user.Password == password)
                {
                    return user.Id;
                }
            }
            return Guid.Empty;
        }

        public DialogsList GetDialogs(Guid userId)
        {
            //прочитать пользователя из файла
            //вернуть список диалогов
            throw new NotImplementedException();
        }

        public MessagesList GetMessages(Guid userId, Guid dialogId, ulong startId, ulong count)
        {
            User user = GetUserById(userId);
            if (!user.DialogIds.Contains(dialogId))
                throw new InvalidOperationException($"Dialog with id {dialogId} does not exists");
            var dialogMesagesDirectory = Path.Combine(MessagesFolder, dialogId.ToString());
            //FIX THIS
            var messageFiles = new DirectoryInfo(dialogMesagesDirectory)
                .GetFiles()
                .OrderBy(f => f.Name) //имена файлов - 1.bin, 2.bin, 3.bin и тд. - хорошо сортируются
                .SkipLast((int)startId).TakeLast((int)count);//Нам нужно взять последние <count> фийлов, начиная с позиции <start>

            MessagesList result = new MessagesList();
            result.DialogId = new GUID { Value = dialogId.ToString() };

            var formatter = new BinaryFormatter();
            foreach (var messageFile in messageFiles)
            {
                using (var stream = messageFile.Open(FileMode.Open))
                {
                    var message = formatter.Deserialize(stream) as Message;

                    if (message.DialogId != dialogId)
                        throw new Exception("DialogId of Message != dialogId");

                    var messageInfo = new MessageInfo
                    {
                        Id = message.Id,
                        SendingTime = message.SendingTime.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                        Text = message.MessageText
                    };

                    result.Messages.Add(messageInfo);
                }
            }

            return result;
        }

        public UserInfo GetUserInfo(Guid userId)
        {
            //прочитать пользователя из файла
            //заполнить нужные поля UserInfo и вернуть объект
            throw new NotImplementedException();
        }

        private static object _registrationLock = new object();
        public Guid Register(RegistrationInfo info)
        {
            //проверить, есть ли юзер с такими Username и Email
            //создать файл и записать в него базовую инфу
            var directory = new DirectoryInfo(UsersFolder);
            var formatter = new BinaryFormatter();
            User newUser;
            //TODO: ADD CHECKS

            lock (_registrationLock)
            {
                foreach (var fileInfo in directory.GetFiles())
                {
                    User user;
                    using (var stream = fileInfo.Open(FileMode.Open))
                    {
                        user = formatter.Deserialize(stream) as User;
                    }

                    if (user.UserName == info.UserName)
                    {
                        throw new ArgumentException("Имя пользователя занято");
                    }
                    if (user.Email == info.Email)
                    {
                        throw new ArgumentException("Пользователь с данным email уже существует");
                    }
                }

                newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Password = info.Password,
                    UserName = info.UserName
                                       ,
                    Age = info.Age,
                    Email = info.Email
                };


                var fileName = Path.Join(UsersFolder, newUser.Id.ToString() + ".bin");
                using (var stream = File.Create(fileName))
                {
                    formatter.Serialize(stream, newUser);
                }
            }

            return newUser.Id;
        }
        static object _messagesLock = new object();
        public void SaveMessage(Guid dialogId, string message)
        {
            //lock(_messagesLock)
            //Проверить существование DialogID
            //посчитать количество файлов в папке сообщений диалога
            //создать сообщение с соответствующим номером в качестве ИД
            //сохранить сообщение в файл
            throw new NotImplementedException();
        }

        public void UpdateUserInfo(Guid userId, UserInfo newInfo)
        {
            //no lock
            throw new NotImplementedException();
        }

        DialogInfo IStorage.CreateDialog(Guid user1, Guid user2)
        {
            //lock (_dialogsLock)
            //прочитать пользователя1
            //проверить список диалогов.
            //если в списке есть диалог с пользователем2 - вернуть его ИД вместе с ИД пользователей
            //создать диалог
            //обновить списки диалогов пользователей 1 и 2
            //вернуть ИД нового диалога вместе с ИД пользователей
            throw new NotImplementedException();
        }
    }
}

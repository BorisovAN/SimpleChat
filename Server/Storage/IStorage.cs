using SimpleChat.Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleChat.Server.Storage
{
    interface IStorage : IDisposable
    {
        /// <summary>
        /// Метод аутентификации
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Возвращает ИД пользователя при успешной аутентификации или Guid.Empty при неудачной</returns>
        Guid Authenticate(string username, string password);

        /// <summary>
        /// Возвращает список диалогов пользователя
        /// </summary>
        /// <param name="userId">ИД  пользователя</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Пользователь не существует</exception>
        DialogsList GetDialogs(Guid userId);

        /// <summary>
        /// Возвращает список сообщений в диалоге
        /// </summary>
        /// <param name="userId">ИД пользователя</param>
        /// <param name="dialogId">ИД диалога</param>
        /// <param name="startId">ИД стартового сообщения</param>
        /// <param name="count">Число сообщений, которые необходимо вернуть</param>
        /// <returns>Сообщения</returns>
        /// <exception cref="InvalidOperationException">Пользователь или диалог не существуют</exception>
        MessagesList GetMessages(Guid userId, Guid dialogId, ulong startId, ulong count);

        /// <summary>
        /// Получает информацию о заданном пользователе
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Пользователь с заданным ИД не существует</exception>
        UserInfo GetUserInfo(Guid userId);

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <param name="info"></param>
        /// <returns>ИД созданного пользователя</returns>
        /// <exception cref="ArgumentException">Пользователь с заданными email и/или username уже существует</exception>
        Guid Register(RegistrationInfo info);


        /// <summary>
        /// Создает диалог для двух заданных пользователей ИЛИ возвращает уже существующий
        /// </summary>
        /// <param name="user1"></param>
        /// <param name="user2"></param>
        /// <returns>ИД диалога</returns>
        /// <exception cref="InvalidOperationException">Один из пользователей не существует</exception>
        DialogInfo CreateDialog(Guid user1, Guid user2);

        /// <summary>
        /// Сохраняет сообщение в заданном диалоге и атоматически выставляет SendingTime
        /// </summary>
        /// <param name="dialogId">ИД диалога</param>
        /// <param name="message">Текст сообщения</param>
        /// <exception cref="InvalidOperationException">Диалог не существует</exception>
        void SaveMessage(Guid dialogId, string message);

        /// <summary>
        /// Обновляет информацию о пользователе
        /// </summary>
        /// <param name="userId">ИД пользователя</param>
        /// <param name="newInfo">Информация</param>
        /// <exception cref="InvalidOperationException">Пользователь с заданным ИД не существует</exception>
        void UpdateUserInfo(Guid userId, UserInfo newInfo);
    }
}

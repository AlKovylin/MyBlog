﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyBlog.Domain.Core;

namespace MyBlog.Infrastructure.Data
{
    public class GenerateData
    {
        private AppDbContext _db { get; set; }

        public GenerateData(AppDbContext db)
        {
            _db = db;
        }

        public void CreateData()
        {
            var roleAdmin = _db.Roles.FirstOrDefault(r => r.Name == "Admin");
            var roleModerator = _db.Roles.FirstOrDefault(r => r.Name == "Moderator");
            var roleUser = _db.Roles.FirstOrDefault(r => r.Name == "User");
            
            var tag1 = new Tag() { Name = "#C#" };
            var tag2 = new Tag() { Name = "#EF" };
            var tag3 = new Tag() { Name = "#ASP.Net" };
            var tag4 = new Tag() { Name = "#MVC" };
            var tag5 = new Tag() { Name = "#Linq" };
            var tag6 = new Tag() { Name = "#SQL" };
            var tag7 = new Tag() { Name = "#HR" };

            _db.Tags.AddRange(tag1, tag2, tag3, tag4, tag5, tag6, tag7);

            var user1 = new User()
            {
                Name = "Иван Николаев",
                Email = "in@ya.ru",
                Password = "11111",
                DisplayName = "Компьютерный ботаник",
                AboutMy = "Увлечённый программист",
            };

            user1.Role.Add(roleUser);

            var user2 = new User()
            {
                Name = "Сергей Петров",
                Email = "sp@ya.ru",
                Password = "11111",
                DisplayName = "Технический гуру",
                AboutMy = "IT специалист с большим опытом",
            };

            user2.Role.Add(roleAdmin);

            var user3 = new User()
            {
                Name = "Виталий Кузнецов",
                Email = "vk@ya.ru",
                Password = "11111",
                DisplayName = "Overgeek",
                AboutMy = "Мастер крутых решений",
            };

            user3.Role.Add(roleModerator);

            var user4 = new User()
            {
                Name = "Егор Чернов",
                Email = "ech@ya.ru",
                Password = "11111",
                DisplayName = "Перезагрузка",
                AboutMy = "HR в IT",
            };

            user4.Role.AddRange(new List<Role>() { roleUser, roleAdmin });

            _db.Users.AddRange(user1, user2, user3, user4);

            var article1 = new Article()
            {
                Title = "Что такое Entity Framework Core",
                Content = "Entity Framework Core (EF Core) представляет собой объектно-ориентированную, легковесную и расширяемую технологию от компании Microsoft " +
                "для доступа к данным. EF Core является ORM-инструментом (object-relational mapping - отображения данных на реальные объекты). То есть EF Core позволяет " +
                "работать базами данных, но представляет собой более высокий уровень абстракции: EF Core позволяет абстрагироваться от самой базы данных и ее таблиц и " +
                "работать с данными независимо от типа хранилища. Если на физическом уровне мы оперируем таблицами, индексами, первичными и внешними ключами, но на " +
                "концептуальном уровне, который нам предлагает Entity Framework, мы уже работаем с объектами.Entity Framework Core поддерживает множество различных систем баз данных." +
                "Таким образом,мы можем через EF Core работать с любой СУБД, если для нее имеется нужный провайдер. По умолчанию на данный момент Microsoft предоставляет " +
                "ряд встроенных провайдеров: для работы с MS SQL Server, для SQLite, для PostgreSQL. Также имеются провайдеры от сторонних поставщиков, например, для MySQL. " +
                "Также стоит отметить, что EF Core предоставляет универсальный API для работы с данными. И если, к примеру, мы решим сменить целевую СУБД, то основные " +
                "изменения в проекте будут касаться прежде всего конфигурации и настройки подключения к соответствующим провайдерам. А код, который непосредственно работает " +
                "с данными, получает данные, добавляет их в БД и т.д., останется прежним. Entity Framework Core многое унаследовал от своих предшественников, в частности, " +
                "Entity Framework 6.В тоже время надо понимать, что EF Core -это не новая версия по отношению к EF 6, а совершенно иная технология, хотя в целом принципы " +
                "работы у них будут совпадать. Поэтому в рамках EF Core используется своя система версий.Текущая версия -5.0 была выпущена в ноябре 2020 года.И технология " +
                "продолжает развиваться."
            };

            article1.Tags.AddRange(new List<Tag>() { tag1, tag2, tag6 });

            var article2 = new Article()
            {
                Title = "Общие сведения ASP.NET Core MVC",
                Content = "ASP.NET MVC является многофункциональной платформой для создания веб-приложений и API-интерфейсов с помощью структуры проектирования " +
                "Model-View-Controller.Структура архитектуры MVC разделяет приложение на три основных группы компонентов: модели, представлении и контроллеры. " +
                "Это позволяет реализовать принципы разделения задач. Согласно этой структуре запросы пользователей направляются в контроллер, который отвечает " +
                "за работу с моделью для выполнения действий пользователей и (или) получение результатов запросов. Контроллер выбирает представление для отображения " +
                "пользователю со всеми необходимыми данными моделиТакое распределение обязанностей позволяет масштабировать приложение в контексте сложности, так как " +
                "проще писать код, выполнять отладку и тестирование компонента (модели, представления или контроллера) с одним заданием. Гораздо труднее обновлять, " +
                "тестировать и отлаживать код, зависимости которого находятся в двух или трех этих областях. Например, логика пользовательского интерфейса, как правило, " +
                "подвергается изменениям чаще, чем бизнес-логика. Если код представления и бизнес-логика объединены в один объект, содержащий бизнес-логику, объект необходимо " +
                "изменять при каждом обновлении пользовательского интерфейса. Это часто приводит к возникновению ошибок и необходимости повторно тестировать бизнес-логику " +
                "после каждого незначительного изменения пользовательского интерфейса.Модель в приложении MVC представляет состояние приложения и бизнес-логику или операций, " +
                "которые должны в нем выполняться. Бизнес-логика должна быть включена в состав модели вместе с логикой реализации для сохранения состояния приложения. " +
                "Как правило, строго типизированные представления используют типы ViewModel, предназначенные для хранения данных, отображаемых в этом представлении. " +
                "Контроллер создает и заполняет эти экземпляры ViewModel из модели."
            };

            article2.Tags.AddRange(new List<Tag>() { tag3, tag4, tag1 });

            var article3 = new Article()
            {
                Title = "LINQ — (C#)",
                Content = "Аббревиатура LINQ обозначает целый набор технологий, создающих и использующих возможности интеграции запросов непосредственно в язык C#. " +
                "Традиционно запросы к данным выражаются в виде простых строк без проверки типов при компиляции или поддержки IntelliSense. Кроме того, разработчику " +
                "приходится изучать различные языки запросов для каждого типа источников данных: баз данных SQL, XML-документов, различных веб-служб и т. д. Технологии " +
                "LINQ превращают запросы в удобную языковую конструкцию, которая применяется аналогично классам, методам и событиям. Вы создаете запросы к строго " +
                "типизированным коллекциям объектов с помощью ключевых слов языка и знакомых операторов. Семейство технологий LINQ обеспечивает согласованное функционирование " +
                "запросов для объектов (LINQ to Objects), реляционных баз данных (LINQ to SQL) и XML (LINQ to XML). Для разработчика, который создает запросы, наиболее " +
                "очевидной частью LINQ является интегрированное выражение запроса.Выражения запроса используют декларативный синтаксис запроса.С помощью синтаксиса запроса " +
                "можно выполнять фильтрацию, упорядочение и группирование данных из источника данных, обходясь минимальным объемом программного кода.Одни и те же базовые выражения " +
                "запроса позволяют запрашивать и преобразовывать данные из баз данных SQL, наборов данных ADO.NET, XML - документов, XML - потоков и коллекций.NET. Вы можете " +
                "писать запросы LINQ в C# для обращения к базам данных SQL Server, XML-документам, наборам данных ADO.NET и любым коллекциям объектов, поддерживающим " +
                "интерфейс IEnumerable или универсальный интерфейс IEnumerable<T>. Кроме того, сторонние разработчики обеспечивают поддержку LINQ для множества веб-служб и " +
                "других реализаций баз данных. В следующем примере показан полный пример использования запроса.Полная операция сначала создает источник данных, затем " +
                "определяет выражение запроса и выполняет этот запрос в инструкции foreach."
            };

            article3.Tags.AddRange(new List<Tag>() { tag1, tag5 });

            var article4 = new Article()
            {
                Title = "И снова LINQ",
                Content = "Выражение запроса можно использовать для получения и преобразования данных из любого источника данных, поддерживающего LINQ. Например, можно одним " +
                "запросом получить данные из базы данных SQL и создать на их основе выходной XML-поток.Аббревиатура LINQ обозначает целый набор технологий, создающих и " +
                "использующих возможности интеграции запросов непосредственно в язык C#. " +
                "Традиционно запросы к данным выражаются в виде простых строк без проверки типов при компиляции или поддержки IntelliSense. Кроме того, разработчику " +
                "приходится изучать различные языки запросов для каждого типа источников данных: баз данных SQL, XML-документов, различных веб-служб и т. д. Технологии " +
                "LINQ превращают запросы в удобную языковую конструкцию, которая применяется аналогично классам, методам и событиям. Вы создаете запросы к строго " +
                "типизированным коллекциям объектов с помощью ключевых слов языка и знакомых операторов. Семейство технологий LINQ обеспечивает согласованное функционирование " +
                "запросов для объектов (LINQ to Objects), реляционных баз данных (LINQ to SQL) и XML (LINQ to XML). Для разработчика, который создает запросы, наиболее " +
                "очевидной частью LINQ является интегрированное выражение запроса.Выражения запроса используют декларативный синтаксис запроса.С помощью синтаксиса запроса " +
                "можно выполнять фильтрацию, упорядочение и группирование данных из источника данных, обходясь минимальным объемом программного кода.Одни и те же базовые выражения " +
                "запроса позволяют запрашивать и преобразовывать данные из баз данных SQL, наборов данных ADO.NET, XML - документов, XML - потоков и коллекций.NET. Вы можете " +
                "писать запросы LINQ в C# для обращения к базам данных SQL Server, XML-документам, наборам данных ADO.NET и любым коллекциям объектов, поддерживающим " +
                "интерфейс IEnumerable или универсальный интерфейс IEnumerable<T>. Кроме того, сторонние разработчики обеспечивают поддержку LINQ для множества веб-служб и " +
                "других реализаций баз данных. В следующем примере показан полный пример использования запроса.Полная операция сначала создает источник данных, затем " +
                "определяет выражение запроса и выполняет этот запрос в инструкции foreach."
            };

            article4.Tags.AddRange(new List<Tag>() { tag1, tag5, tag2 });

            var article5 = new Article()
            {
                Title = "«Весь» HR в IT на одной схеме",
                Content = "Неважно, какого масштаба ваш бизнес, без HR-CRM системы не обойтись. На рынке предложено множество облачных вариантов (E-Staff, Huntflow, " +
                "Talantix и т.д.), которые вполне подойдут для стартапов. А вот большие и средние компании не могут позволить себе даже незначительный риск утечки " +
                "самого ценного в HR. Поэтому им стоит рассматривать использование on-premise систем. Кадровое планирование должно учитывать органичный рост команд.Без этого " +
                "можно испортить даже хорошего специалиста, погрузив в команду, у которой в текущий момент нет ресурсов на его обучение. Для поиска кандидатов всегда " +
                "необходимо формировать четкий профиль, чтобы не тратить время как рекрутера, так и технических специалистов на заведомо неподходящих кандидатов. Важно " +
                "обучить HR - специалиста стеку и взаимозаменяемым технологиям для того, чтобы не пропустить полезных специалистов.Отучить рекрутеров от применения речевых " +
                "модулей, т.к.шаблонные фразы создают негативный эффект. Наём — классическая сделка.Навыки рекрутера должны включать исследование потребностей, работу с " +
                "возражениями и продажу HR - бренда. Если вы стартап, то заниматься HR-брендингом не имеет смысла, разве что это какой-нибудь «фантастический единорог». Но " +
                "тогда ваше название и так у всех на слуху. У средних компаний возможности достаточно ограничены.Если вы не хотите потеряться на фоне остальных игроков локального " +
                "рынка, необходимо проведение митапов, хакатонов и других активностей, включая выступления на профильных конференциях, создание электронных курсов обучения, " +
                "работу с представителями местных ИТ - сообществ и пр"
            };

            article5.Tags.AddRange(new List<Tag>() { tag7 });

            _db.Articles.AddRange(article1, article2, article3, article4, article5);

            user1.Article.AddRange(new List<Article>() { article1, article2, article3 });
            user4.Article.AddRange(new List<Article>() { article4, article5 });

            var comment1 = new Comment() { Content = "Очень интересная статья.", User = user2 };
            var comment2 = new Comment() { Content = "Напишите ещё об этом.", User = user3 };

            _db.Comments.AddRange(comment1, comment2);

            article1.Comment.AddRange(new List<Comment>() { comment1, comment2 });

            _db.SaveChanges();
        }
    }
}
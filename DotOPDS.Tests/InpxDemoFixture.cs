﻿using System;
using System.Collections.Generic;
using DotOPDS.Models;

namespace DotOPDS.Tests
{
    class InpxDemoFixture
    {
        public static List<Book> Result = new List<Book>
        {
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Владимир", MiddleName = "Казимирович", LastName = "Венгловский"}
                },
                Genres = new[] {
                    "sf_heroic",
                    "sf_cyberpunk",
                    "sf"
                },
                Title = "Хардкор",
                Series = null,
                SeriesNo = 0,
                File = "492166",
                Size = 1539547,
                LibId = 492166,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Сергей", MiddleName = null, LastName = "Челяев"},
                    new Author {FirstName = "Александр", MiddleName = null, LastName = "Зорич"}
                },
                Genres = new[] {
                    "sf_action"
                },
                Title = "Клад Стервятника",
                Series = "Комбат и Тополь",
                SeriesNo = 3,
                File = "492167",
                Size = 1156638,
                LibId = 492167,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Владислав", MiddleName = "Валерьевич", LastName = "Выставной"}
                },
                Genres = new[] {
                    "sf_action"
                },
                Title = "Убить зону",
                Series = "Бука",
                SeriesNo = 1,
                File = "492168",
                Size = 1185675,
                LibId = 492168,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Леонид", MiddleName = "Викторович", LastName = "Кудрявцев"}
                },
                Genres = new[] {
                    "sf_action"
                },
                Title = "Пуля для контролера",
                Series = null,
                SeriesNo = 80,
                File = "492169",
                Size = 1218213,
                LibId = 492169,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Дмитрий", MiddleName = "Александрович", LastName = "Тихонов"}
                },
                Genres = new[] {
                    "sf_fantasy"
                },
                Title = "Эпоха последних слов",
                Series = null,
                SeriesNo = 0,
                File = "492170",
                Size = 1655936,
                LibId = 492170,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Наталия", MiddleName = "Юрьевна", LastName = "Вико"}
                },
                Genres = new[] {
                    "prose_contemporary"
                },
                Title = "Шизофрения",
                Series = null,
                SeriesNo = -1,
                File = "492174",
                Size = 1224632,
                LibId = 492174,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Михаил", MiddleName = "Аркадьевич", LastName = "Шуваев"}
                },
                Genres = new[] {
                    "sf_space"
                },
                Title = "Лунная соната",
                Series = "Звездный Гольфстрим",
                SeriesNo = 2,
                File = "492175",
                Size = 1072976,
                LibId = 492175,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Ксения", MiddleName = null, LastName = "Лазорева"}
                },
                Genres = new[] {
                    "sf_fantasy"
                },
                Title = "Сирота с Севера. История Кальвина Рейвена[СИ]",
                Series = "Легенда о Слепых Богах",
                SeriesNo = 0,
                File = "492179",
                Size = 122289,
                LibId = 492179,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Дмитрий", MiddleName = "Сергеевич", LastName = "Бирюков"}
                },
                Genres = new[] {
                    "sf_social",
                    "prose_contemporary"
                },
                Title = "Снежный Горький и много воды",
                Series = null,
                SeriesNo = -1,
                File = "492180",
                Size = 64150,
                LibId = 492180,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Михаил", MiddleName = "Анатольевич", LastName = "Голденков"}
                },
                Genres = new[] {
                    "prose_history",
                    "adv_history"
                },
                Title = "Тропою волка",
                Series = "Пан Кмитич",
                SeriesNo = 2,
                File = "492183",
                Size = 5154918,
                LibId = 492183,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Питер", MiddleName = null, LastName = "Фехервари"}
                },
                Genres = new[] {
                    "sf_epic"
                },
                Title = "Змеев заповедник",
                Series = "Warhammer 40000: Охотники на ксеносов",
                SeriesNo = 8,
                File = "492184",
                Size = 464671,
                LibId = 492184,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Сергей", MiddleName = "Васильевич", LastName = "Лукьяненко"}
                },
                Genres = new[] {
                    "sf"
                },
                Title = "За лісом, де підлий ворог",
                Series = null,
                SeriesNo = -1,
                File = "492204",
                Size = 51739,
                LibId = 492204,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Линдсей", MiddleName = null, LastName = "Дэвис"}
                },
                Genres = new[] {
                    "det_history"
                },
                Title = "Enemies at Home",
                Series = "Flavia Albia mystery",
                SeriesNo = 2,
                File = "492205",
                Size = 678510,
                LibId = 492205,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Pierre", MiddleName = null, LastName = "Dac"}
                },
                Genres = new[] {
                    "poetry",
                    "dramaturgy",
                    "nonf_biography",
                    "nonf_publicism",
                    "humor_prose"
                },
                Title = "Dico franco-loufoque",
                Series = null,
                SeriesNo = 128,
                File = "492208",
                Size = 282032,
                LibId = 492208,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Грегори", MiddleName = null, LastName = "Хаус"}
                },
                Genres = new[] {
                    "det_history"
                },
                Title = "The Liberties of London",
                Series = "Red Ned Tudor Mysteries",
                SeriesNo = 1,
                File = "492209",
                Size = 286525,
                LibId = 492209,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Грегори", MiddleName = null, LastName = "Хаус"}
                },
                Genres = new[] {
                    "det_history"
                },
                Title = "The Fetter Lane Fleece",
                Series = "Red Ned Tudor Mysteries",
                SeriesNo = 2,
                File = "492210",
                Size = 154495,
                LibId = 492210,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Грегори", MiddleName = null, LastName = "Хаус"}
                },
                Genres = new[] {
                    "det_history"
                },
                Title = "The Queen's Oranges",
                Series = "Red Ned Tudor Mysteries",
                SeriesNo = 3,
                File = "492211",
                Size = 833670,
                LibId = 492211,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Грегори", MiddleName = null, LastName = "Хаус"}
                },
                Genres = new[] {
                    "det_history"
                },
                Title = "The Cardinal's Angels",
                Series = "Red Ned Tudor Mysteries",
                SeriesNo = 4,
                File = "492212",
                Size = 677020,
                LibId = 492212,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Грегори", MiddleName = null, LastName = "Хаус"}
                },
                Genres = new[] {
                    "det_history"
                },
                Title = "A Comfit Of Rogues",
                Series = "Red Ned Tudor Mysteries",
                SeriesNo = 5,
                File = "492213",
                Size = 258343,
                LibId = 492213,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Грегори", MiddleName = null, LastName = "Хаус"}
                },
                Genres = new[] {
                    "det_history"
                },
                Title = "The Lord Of Misrule",
                Series = "Red Ned Tudor Mysteries",
                SeriesNo = 6,
                File = "492214",
                Size = 300328,
                LibId = 492214,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Ian", MiddleName = null, LastName = "Miller"}
                },
                Genres = new[] {
                    "adv_history"
                },
                Title = "Athene's prophesy",
                Series = "Gaius Claudius Scaevola",
                SeriesNo = 1,
                File = "492215",
                Size = 869518,
                LibId = 492215,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Ian", MiddleName = null, LastName = "Miller"}
                },
                Genres = new[] {
                    "adv_history"
                },
                Title = "Legatus Legionis",
                Series = "Gaius Claudius Scaevola",
                SeriesNo = 2,
                File = "492216",
                Size = 863933,
                LibId = 492216,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Артур", MiddleName = "Чарльз", LastName = "Кларк"}
                },
                Genres = new[] {
                    "sf"
                },
                Title = "Фонтаны рая",
                Series = null,
                SeriesNo = 1980,
                File = "492223",
                Size = 579752,
                LibId = 492223,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Antonio", MiddleName = null, LastName = "Hill"}
                },
                Genres = new[] {
                    "thriller"
                },
                Title = "The Summer of Dead Toys",
                Series = "Inspector Hector Salgado",
                SeriesNo = 1,
                File = "492225",
                Size = 565384,
                LibId = 492225,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Antonio", MiddleName = null, LastName = "Hill"}
                },
                Genres = new[] {
                    "thriller"
                },
                Title = "The Good Suicides",
                Series = "Inspector Hector Salgado",
                SeriesNo = 2,
                File = "492226",
                Size = 925284,
                LibId = 492226,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Лаура", MiddleName = null, LastName = "Паркер"}
                },
                Genres = new[] {
                    "love_history"
                },
                Title = "Отвергнутая",
                Series = "Masqueraders",
                SeriesNo = 3,
                File = "492227",
                Size = 1089638,
                LibId = 492227,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Стэйси", MiddleName = null, LastName = "Кейд"}
                },
                Genres = new[] {
                    "love_sf",
                    "sf_mystic"
                },
                Title = "Призрак и гот",
                Series = "Призрак и гот",
                SeriesNo = 1,
                File = "492228",
                Size = 721302,
                LibId = 492228,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Victor", MiddleName = null, LastName = "Methos"}
                },
                Genres = new[] {
                    "thriller"
                },
                Title = "Sin City Homicide",
                Series = "Jon Stanton",
                SeriesNo = 3,
                File = "492229",
                Size = 541037,
                LibId = 492229,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Роберто", MiddleName = null, LastName = "Калассо"}
                },
                Genres = new[] {
                    "prose_contemporary",
                    "nonf_publicism"
                },
                Title = "Ka",
                Series = null,
                SeriesNo = -1,
                File = "492231",
                Size = 2059394,
                LibId = 492231,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Екатерина", MiddleName = "Васильевна", LastName = "Васичкина"}
                },
                Genres = new[] {
                    "love_sf"
                },
                Title = "Лада все сладит",
                Series = null,
                SeriesNo = -1,
                File = "492232",
                Size = 115531,
                LibId = 492232,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Екатерина", MiddleName = "Васильевна", LastName = "Васичкина"}
                },
                Genres = new[] {
                    "sf_fantasy"
                },
                Title = "Линда",
                Series = null,
                SeriesNo = -1,
                File = "492233",
                Size = 416781,
                LibId = 492233,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Екатерина", MiddleName = "Васильевна", LastName = "Васичкина"}
                },
                Genres = new[] {
                    "love_sf"
                },
                Title = "Про любовь",
                Series = null,
                SeriesNo = -1,
                File = "492234",
                Size = 70125,
                LibId = 492234,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Екатерина", MiddleName = "Васильевна", LastName = "Васичкина"}
                },
                Genres = new[] {
                    "sf_fantasy"
                },
                Title = "Ученик",
                Series = null,
                SeriesNo = -1,
                File = "492235",
                Size = 177132,
                LibId = 492235,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Екатерина", MiddleName = "Васильевна", LastName = "Васичкина"}
                },
                Genres = new[] {
                    "sf_fantasy"
                },
                Title = "Верность",
                Series = null,
                SeriesNo = -1,
                File = "492236",
                Size = 88012,
                LibId = 492236,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf_social"
                },
                Title = "Анатомия абсурда",
                Series = null,
                SeriesNo = -1,
                File = "492238",
                Size = 343618,
                LibId = 492238,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf"
                },
                Title = "Бестиарий",
                Series = null,
                SeriesNo = -1,
                File = "492239",
                Size = 174998,
                LibId = 492239,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf_horror"
                },
                Title = "Человек помер",
                Series = null,
                SeriesNo = -1,
                File = "492240",
                Size = 158677,
                LibId = 492240,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf_horror"
                },
                Title = "Это не мой труп",
                Series = null,
                SeriesNo = -1,
                File = "492241",
                Size = 196501,
                LibId = 492241,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf_space"
                },
                Title = "Красная нить",
                Series = null,
                SeriesNo = -1,
                File = "492242",
                Size = 222478,
                LibId = 492242,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf"
                },
                Title = "Малеевский феномен",
                Series = null,
                SeriesNo = -1,
                File = "492243",
                Size = 185133,
                LibId = 492243,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf_social"
                },
                Title = "О чем молчала бабушка",
                Series = null,
                SeriesNo = -1,
                File = "492244",
                Size = 180582,
                LibId = 492244,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf_horror"
                },
                Title = "Охотники за именами",
                Series = null,
                SeriesNo = -1,
                File = "492245",
                Size = 174713,
                LibId = 492245,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf_horror"
                },
                Title = "Он все время уходит",
                Series = null,
                SeriesNo = -1,
                File = "492246",
                Size = 207101,
                LibId = 492246,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf_space"
                },
                Title = "Патриоты",
                Series = null,
                SeriesNo = -1,
                File = "492247",
                Size = 189120,
                LibId = 492247,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf_space"
                },
                Title = "Побеждающий разум",
                Series = null,
                SeriesNo = -1,
                File = "492248",
                Size = 188654,
                LibId = 492248,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf"
                },
                Title = "Пожирающий разом",
                Series = null,
                SeriesNo = -1,
                File = "492249",
                Size = 196010,
                LibId = 492249,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf_horror"
                },
                Title = "Произнесенное вслух",
                Series = null,
                SeriesNo = -1,
                File = "492250",
                Size = 175927,
                LibId = 492250,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf_space"
                },
                Title = "Щупальце",
                Series = null,
                SeriesNo = -1,
                File = "492251",
                Size = 160013,
                LibId = 492251,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf_horror"
                },
                Title = "Смерть Родриго Нуньеса",
                Series = null,
                SeriesNo = -1,
                File = "492252",
                Size = 172543,
                LibId = 492252,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf_horror"
                },
                Title = "Солнечные зайчики",
                Series = null,
                SeriesNo = -1,
                File = "492253",
                Size = 186219,
                LibId = 492253,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf_horror"
                },
                Title = "Тень розы",
                Series = null,
                SeriesNo = -1,
                File = "492254",
                Size = 173395,
                LibId = 492254,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf"
                },
                Title = "Третий дождь",
                Series = null,
                SeriesNo = -1,
                File = "492255",
                Size = 214479,
                LibId = 492255,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf_social"
                },
                Title = "В лабиринтах Инфора",
                Series = null,
                SeriesNo = -1,
                File = "492256",
                Size = 414501,
                LibId = 492256,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Иван", MiddleName = "Кузьмич", LastName = "Андрощук"}
                },
                Genres = new[] {
                    "sf_space"
                },
                Title = "Звездные волки",
                Series = null,
                SeriesNo = -1,
                File = "492257",
                Size = 181831,
                LibId = 492257,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Хироси", MiddleName = null, LastName = "Сакурадзака"}
                },
                Genres = new[] {
                    "sf_action",
                    "sf"
                },
                Title = "Грань будущего",
                Series = null,
                SeriesNo = -1,
                File = "492258",
                Size = 977762,
                LibId = 492258,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Гай Валерий", MiddleName = null, LastName = "Катулл"}
                },
                Genres = new[] {
                    "poetry",
                    "antique_ant"
                },
                Title = "33 стихотворения",
                Series = null,
                SeriesNo = -1,
                File = "492259",
                Size = 41092,
                LibId = 492259,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Николай", MiddleName = "Александрович", LastName = "Верещагин"}
                },
                Genres = new[] {
                    "prose_su_classics",
                    "love_contemporary"
                },
                Title = "Приключение",
                Series = null,
                SeriesNo = 0,
                File = "492260",
                Size = 173338,
                LibId = 492260,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Кара", MiddleName = null, LastName = "Эллиот"}
                },
                Genres = new[] {
                    "love_history"
                },
                Title = "Не в силах устоять",
                Series = "Лорды полуночи",
                SeriesNo = 2,
                File = "492261",
                Size = 897725,
                LibId = 492261,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Джасинда", MiddleName = null, LastName = "Уайлдер"}
                },
                Genres = new[] {
                    "love_contemporary"
                },
                Title = "Я, ты и любовь",
                Series = "Falling Into",
                SeriesNo = 1,
                File = "492262",
                Size = 832158,
                LibId = 492262,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Джеральд", MiddleName = null, LastName = "Даррелл"}
                },
                Genres = new[] {
                    "adv_animal"
                },
                Title = "A Zoo in My Luggage",
                Series = null,
                SeriesNo = -1,
                File = "492263",
                Size = 2913796,
                LibId = 492263,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Яков", MiddleName = null, LastName = "Шехтер"}
                },
                Genres = new[] {
                    "prose_contemporary"
                },
                Title = "Любовь и СМЕРШ",
                Series = null,
                SeriesNo = -1,
                File = "492264",
                Size = 1073157,
                LibId = 492264,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Клаус", MiddleName = null, LastName = "Манн"}
                },
                Genres = new[] {
                    "nonf_biography"
                },
                Title = "На повороте. Жизнеописание",
                Series = null,
                SeriesNo = -1,
                File = "492265",
                Size = 4464680,
                LibId = 492265,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Умберто", MiddleName = null, LastName = "Эко"},
                    new Author {FirstName = "Жан-Клод", MiddleName = null, LastName = "Карьер"}
                },
                Genres = new[] {
                    "sci_culture",
                    "sci_philosophy",
                    "nonf_publicism",
                    "nonf_criticism"
                },
                Title = "N'espérez pas vous débarrasser des livres",
                Series = null,
                SeriesNo = 0,
                File = "492266",
                Size = 552688,
                LibId = 492266,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Александр", MiddleName = "Петрович", LastName = "Коцюбинский"},
                    new Author {FirstName = "Даниил", MiddleName = "А", LastName = "Коцюбинский"}
                },
                Genres = new[] {
                    "sci_history"
                },
                Title = "Распутин. Жизнь. Смерть. Тайна",
                Series = null,
                SeriesNo = -1,
                File = "492267",
                Size = 4622047,
                LibId = 492267,
                Del = true,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Лоранс", MiddleName = null, LastName = "Левассер"}
                },
                Genres = new[] {
                    "sci_psychology",
                    "religion_self",
                    "home"
                },
                Title = "50 упражнений для развития способности жить настоящим",
                Series = "Психология. Прорыв",
                SeriesNo = 0,
                File = "492269",
                Size = 1590136,
                LibId = 492269,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Михаил", MiddleName = "Анатольевич", LastName = "Голденков"}
                },
                Genres = new[] {
                    "prose_history",
                    "adv_history"
                },
                Title = "Схватка",
                Series = "Пан Кмитич",
                SeriesNo = 3,
                File = "492270",
                Size = 5429293,
                LibId = 492270,
                Del = true,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Игорь", MiddleName = "Витальевич", LastName = "Шелег"}
                },
                Genres = new[] {
                    "sf_fantasy"
                },
                Title = "Нужно просто остаться в живых",
                Series = "Нужно просто остаться в живых",
                SeriesNo = 1,
                File = "492271",
                Size = 689897,
                LibId = 492271,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Юлия", MiddleName = "Анатольевна", LastName = "Панченко"}
                },
                Genres = new[] {
                    "sf_fantasy",
                    "love_sf"
                },
                Title = "Ненавижу, слышишь? И люблю...",
                Series = null,
                SeriesNo = -1,
                File = "492274",
                Size = 730859,
                LibId = 492274,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = null, MiddleName = null, LastName = "Маркоша"}
                },
                Genres = new[] {
                    "sf_space",
                    "sf_etc"
                },
                Title = "Пазл",
                Series = null,
                SeriesNo = -1,
                File = "492275",
                Size = 915569,
                LibId = 492275,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Фред", MiddleName = null, LastName = "Стюарт"}
                },
                Genres = new[] {
                    "sagas"
                },
                Title = "Золото и мишура",
                Series = null,
                SeriesNo = 0,
                File = "492277",
                Size = 2430424,
                LibId = 492277,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Николай", MiddleName = "Владимирович", LastName = "Белов"}
                },
                Genres = new[] {
                    "nonf_biography"
                },
                Title = "Звезда Монро",
                Series = null,
                SeriesNo = 0,
                File = "492278",
                Size = 798338,
                LibId = 492278,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Эндрю", MiddleName = null, LastName = "О'Хоган"}
                },
                Genres = new[] {
                    "prose_contemporary"
                },
                Title = "Взгляды на жизнь щенка Мафа и его хозяйки — Мэрилин Монро",
                Series = null,
                SeriesNo = -1,
                File = "492280",
                Size = 981509,
                LibId = 492280,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Арлен", MiddleName = null, LastName = "Аир"}
                },
                Genres = new[] {
                    "sf_fantasy"
                },
                Title = "Кружевница",
                Series = null,
                SeriesNo = -1,
                File = "492290",
                Size = 627385,
                LibId = 492290,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Владимир", MiddleName = null, LastName = "Махов"}
                },
                Genres = new[] {
                    "sf_action",
                    "adv_maritime"
                },
                Title = "Декомпрессия",
                Series = "Океан (фантастика)",
                SeriesNo = 2,
                File = "492291",
                Size = 929401,
                LibId = 492291,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Хантер", MiddleName = "С", LastName = "Томпсон"}
                },
                Genres = new[] {
                    "prose_counter"
                },
                Title = "Страх и отвращение в Лас-Вегасе",
                Series = null,
                SeriesNo = -1,
                File = "492294",
                Size = 368259,
                LibId = 492294,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Майя", MiddleName = "Юрьевна", LastName = "Горина"}
                },
                Genres = new[] {
                    "religion_esoterics"
                },
                Title = "Статьи Майи Гориной, ч. 3",
                Series = null,
                SeriesNo = -1,
                File = "492300",
                Size = 4074233,
                LibId = 492300,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Майя", MiddleName = "Юрьевна", LastName = "Горина"}
                },
                Genres = new[] {
                    "religion_esoterics"
                },
                Title = "Копилка Мудрости от Творца",
                Series = null,
                SeriesNo = -1,
                File = "492303",
                Size = 117372,
                LibId = 492303,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Роберт", MiddleName = "Ирвин", LastName = "Говард"}
                },
                Genres = new[] {
                    "sf_fantasy"
                },
                Title = "Конан. Рожденный в битве",
                Series = "Конан. Сборники",
                SeriesNo = 1,
                File = "492306",
                Size = 1082946,
                LibId = 492306,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Роберт", MiddleName = "Ирвин", LastName = "Говард"}
                },
                Genres = new[] {
                    "sf_fantasy"
                },
                Title = "Конан. Кровавый венец",
                Series = "Конан. Сборники",
                SeriesNo = 2,
                File = "492307",
                Size = 890924,
                LibId = 492307,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Роберт", MiddleName = "Ирвин", LastName = "Говард"}
                },
                Genres = new[] {
                    "sf_fantasy"
                },
                Title = "Конан. Карающий меч",
                Series = "Конан. Сборники",
                SeriesNo = 3,
                File = "492308",
                Size = 886397,
                LibId = 492308,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Дэвид", MiddleName = null, LastName = "Брин"},
                    new Author {FirstName = "Брюс", MiddleName = null, LastName = "Стерлинг"},
                    new Author {FirstName = "Алексис", MiddleName = "де", LastName = "Токвиль"},
                    new Author {FirstName = null, MiddleName = null, LastName = "Журнал «Сверхновая американская фантастика»"},
                    new Author {FirstName = "Дин", MiddleName = null, LastName = "Уитлок"},
                    new Author {FirstName = "Майк", MiddleName = null, LastName = "Коннер"},
                    new Author {FirstName = "Уэйн", MiddleName = null, LastName = "Уайтмэн"},
                    new Author {FirstName = "Элизабет", MiddleName = null, LastName = "Краус"}
                },
                Genres = new[] {
                    "sf",
                    "periodic"
                },
                Title = "Сверхновая американская фантастика, 1995 № 05-06",
                Series = null,
                SeriesNo = 11,
                File = "492311",
                Size = 858200,
                LibId = 492311,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Крессида", MiddleName = null, LastName = "Коуэлл"}
                },
                Genres = new[] {
                    "child_sf"
                },
                Title = "Как пережить штурм дракона",
                Series = "Как приручить дракона",
                SeriesNo = 7,
                File = "492314",
                Size = 2671399,
                LibId = 492314,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Крессида", MiddleName = null, LastName = "Коуэлл"}
                },
                Genres = new[] {
                    "child_sf"
                },
                Title = "Как приручить викинга",
                Series = "Как приручить дракона",
                SeriesNo = 6,
                File = "492315",
                Size = 526796,
                LibId = 492315,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Крессида", MiddleName = null, LastName = "Коуэлл"}
                },
                Genres = new[] {
                    "child_sf",
                    "child_adv"
                },
                Title = "Как перехитрить дракона",
                Series = "Как приручить дракона",
                SeriesNo = 4,
                File = "492316",
                Size = 5150431,
                LibId = 492316,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Крессида", MiddleName = null, LastName = "Коуэлл"}
                },
                Genres = new[] {
                    "child_sf"
                },
                Title = "Как приручить викинга",
                Series = "Как приручить дракона",
                SeriesNo = 6,
                File = "492318",
                Size = 501522,
                LibId = 492318,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Крессида", MiddleName = null, LastName = "Коуэлл"}
                },
                Genres = new[] {
                    "child_sf"
                },
                Title = "Как переиграть историю дракона",
                Series = "Как приручить дракона",
                SeriesNo = 5,
                File = "492319",
                Size = 225587,
                LibId = 492319,
                Del = true,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Евгений", MiddleName = "Алексеевич", LastName = "Обухов"},
                    new Author {FirstName = "Леонид", MiddleName = "Викторович", LastName = "Кудрявцев"},
                    new Author {FirstName = null, MiddleName = null, LastName = "Журнал «Космопорт»"},
                    new Author {FirstName = "Александр", MiddleName = "Владимирович", LastName = "Марков"},
                    new Author {FirstName = "Майк", MiddleName = null, LastName = "Гелприн"},
                    new Author {FirstName = "Сергей", MiddleName = "Алексеевич", LastName = "Булыга"},
                    new Author {FirstName = "Сергей", MiddleName = "Валериевич", LastName = "Легеза"},
                    new Author {FirstName = "Илья", MiddleName = null, LastName = "Суханов"}
                },
                Genres = new[] {
                    "sf",
                    "periodic"
                },
                Title = "Космопорт, 2013 № 01",
                Series = "Космопорт (журнал)",
                SeriesNo = 1,
                File = "492322",
                Size = 737271,
                LibId = 492322,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Евгений", MiddleName = "Ануфриевич", LastName = "Дрозд"},
                    new Author {FirstName = "Дмитрий", MiddleName = "Станиславович", LastName = "Федотов"},
                    new Author {FirstName = "Яцек", MiddleName = null, LastName = "Савашкевич"},
                    new Author {FirstName = null, MiddleName = null, LastName = "Журнал «Космопорт»"},
                    new Author {FirstName = "Сергей", MiddleName = "Валериевич", LastName = "Легеза"},
                    new Author {FirstName = "Святослав", MiddleName = "Владимирович", LastName = "Логинов"},
                    new Author {FirstName = "Александр", MiddleName = "Александрович", LastName = "Змушко"},
                    new Author {FirstName = "Клиффорд", MiddleName = null, LastName = "Болл"},
                    new Author {FirstName = "Анна", MiddleName = null, LastName = "Бжезинская"},
                    new Author {FirstName = "Владимир", MiddleName = "Казимирович", LastName = "Венгловский"}
                },
                Genres = new[] {
                    "sf",
                    "periodic"
                },
                Title = "Космопорт, 2014 № 01 (2)",
                Series = "Космопорт (журнал)",
                SeriesNo = 2,
                File = "492323",
                Size = 1004736,
                LibId = 492323,
                Del = true,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Евгений", MiddleName = "Ануфриевич", LastName = "Дрозд"},
                    new Author {FirstName = "Леонид", MiddleName = "Викторович", LastName = "Кудрявцев"},
                    new Author {FirstName = null, MiddleName = null, LastName = "Журнал «Космопорт»"},
                    new Author {FirstName = "Олег", MiddleName = null, LastName = "Костенко"},
                    new Author {FirstName = "Екатерина", MiddleName = "Дмитриевна", LastName = "Белоусова"},
                    new Author {FirstName = "Сергей", MiddleName = "Алексеевич", LastName = "Булыга"},
                    new Author {FirstName = "Андрей", MiddleName = "Васильевич", LastName = "Саломатов"},
                    new Author {FirstName = "Екатерина", MiddleName = null, LastName = "Гракова"},
                    new Author {FirstName = "Александр", MiddleName = null, LastName = "Рыжков"},
                    new Author {FirstName = "Анатолий", MiddleName = null, LastName = "Белиловский"},
                    new Author {FirstName = "Михаил", MiddleName = null, LastName = "Ифф"},
                    new Author {FirstName = "Константин", MiddleName = null, LastName = "Чихунов"}
                },
                Genres = new[] {
                    "sf",
                    "periodic"
                },
                Title = "Космопорт, 2014 № 04 (5)",
                Series = "Космопорт (журнал)",
                SeriesNo = 5,
                File = "492324",
                Size = 563742,
                LibId = 492324,
                Del = true,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Виктория", MiddleName = "Владимировна", LastName = "Карпухина"}
                },
                Genres = new[] {
                    "home_cooking",
                    "home_health"
                },
                Title = "Энциклопедия целительных специй. Имбирь, куркума, кориандр, корица, шафран и еще 100 исцеляющих специй",
                Series = null,
                SeriesNo = 0,
                File = "492327",
                Size = 734041,
                LibId = 492327,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Ариадна", MiddleName = "Валентиновна", LastName = "Борисова"}
                },
                Genres = new[] {
                    "love_contemporary"
                },
                Title = "Когда вырастают дети",
                Series = null,
                SeriesNo = -1,
                File = "492328",
                Size = 1106939,
                LibId = 492328,
                Del = true,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Татьяна", MiddleName = "Михайловна", LastName = "Тронина"}
                },
                Genres = new[] {
                    "love_detective"
                },
                Title = "Та, кто приходит незваной",
                Series = null,
                SeriesNo = -1,
                File = "492329",
                Size = 1192013,
                LibId = 492329,
                Del = true,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Александр", MiddleName = "Петрович", LastName = "Коцюбинский"},
                    new Author {FirstName = "Даниил", MiddleName = "А", LastName = "Коцюбинский"}
                },
                Genres = new[] {
                    "nonf_biography"
                },
                Title = "Распутин. Жизнь. Смерть. Тайна",
                Series = null,
                SeriesNo = -1,
                File = "492330",
                Size = 4733416,
                LibId = 492330,
                Del = true,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Фиона", MiddleName = null, LastName = "Харпер"}
                },
                Genres = new[] {
                    "love_short"
                },
                Title = "Хитрости любви",
                Series = null,
                SeriesNo = 394,
                File = "492334",
                Size = 658650,
                LibId = 492334,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Александр", MiddleName = null, LastName = "Костенко"}
                },
                Genres = new[] {
                    "sf_horror",
                    "det_history",
                    "thriller"
                },
                Title = "Интересно девки пляшут, или Введение в профессию",
                Series = null,
                SeriesNo = -1,
                File = "492336",
                Size = 1088306,
                LibId = 492336,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Ефим", MiddleName = "Яковлевич", LastName = "Курганов"}
                },
                Genres = new[] {
                    "det_history"
                },
                Title = "Воры над законом, или Дело Политковского",
                Series = "Старая уголовная хроника",
                SeriesNo = 0,
                File = "492337",
                Size = 951796,
                LibId = 492337,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Елена", MiddleName = null, LastName = "Вос"}
                },
                Genres = new[] {
                    "ref_ref",
                    "religion_self"
                },
                Title = "Этикет в ресторане",
                Series = null,
                SeriesNo = -1,
                File = "492338",
                Size = 377811,
                LibId = 492338,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Капка", MiddleName = null, LastName = "Кассабова"}
                },
                Genres = new[] {
                    "nonf_biography"
                },
                Title = "Двенадцать минут любви",
                Series = null,
                SeriesNo = -1,
                File = "492358",
                Size = 1181041,
                LibId = 492358,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "А", MiddleName = "Дж", LastName = "Моллой"}
                },
                Genres = new[] {
                    "love_detective",
                    "love_erotica"
                },
                Title = "История Икс",
                Series = null,
                SeriesNo = 0,
                File = "492359",
                Size = 1296847,
                LibId = 492359,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("18.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Валерий", MiddleName = "Сергеевич", LastName = "Флёров"}
                },
                Genres = new[] {
                    "sci_history"
                },
                Title = "«Города» и «замки» Хазарского каганата. Археологическая реальность",
                Series = null,
                SeriesNo = -1,
                File = "492372",
                Size = 4041007,
                LibId = 492372,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("19.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            },
            new Book {
                Authors = new[] {
                    new Author {FirstName = "Роберто", MiddleName = null, LastName = "Калассо"}
                },
                Genres = new[] {
                    "prose_contemporary",
                    "nonf_publicism",
                    "nonf_criticism"
                },
                Title = "Literature and the Gods",
                Series = null,
                SeriesNo = -1,
                File = "492374",
                Size = 1898662,
                LibId = 492374,
                Del = false,
                Ext = "fb2",
                Date = DateTime.Parse("19.06.2014 0:00:00"),
                Archive = "fb2-492166-492374_lost.zip"
            }
        };
    }
}

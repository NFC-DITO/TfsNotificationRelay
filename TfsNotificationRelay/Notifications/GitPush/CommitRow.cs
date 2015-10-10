﻿/*
 * TfsNotificationRelay - http://github.com/kria/TfsNotificationRelay
 * 
 * Copyright (C) 2014 Kristian Adrup
 * 
 * This file is part of TfsNotificationRelay.
 * 
 * TfsNotificationRelay is free software: you can redistribute it and/or 
 * modify it under the terms of the GNU General Public License as published 
 * by the Free Software Foundation, either version 3 of the License, or 
 * (at your option) any later version. See included file COPYING for details.
 */

using DevCore.TfsNotificationRelay.Configuration;
using Microsoft.TeamFoundation.Git.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevCore.TfsNotificationRelay.Notifications.GitPush
{
    public class CommitRow : NotificationRow
    {
        public Sha1Id CommitId { get; set; }
        public CommitRowType Type { get; set; }
        public string CommitUri { get; set; }
        public Dictionary<TfsGitChangeType, int> ChangeCounts { get; set; }
        public DateTime AuthorTime { get; set; }
        public string Author { get; set; }
        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }
        public string Comment { get; set; }
        public IEnumerable<GitRef> Refs { get; set; }

        public override string ToString(BotElement bot, Func<string, string> transform)
        {
            string formattedTime = string.IsNullOrEmpty(bot.Text.DateTimeFormat) ? AuthorTime.ToString() : AuthorTime.ToString(bot.Text.DateTimeFormat);
            var sb = new StringBuilder();
            if (Refs != null) sb.AppendFormat("{0} ", Refs.ToString(bot, transform));
            
            sb.Append(bot.Text.CommitFormat.FormatWith(new
            {
                Action = Type == CommitRowType.Commit ? bot.Text.Commit : bot.Text.RefPointer, CommitUri,
                CommitId = transform(CommitId.ToHexString(Settings.HashLength)),
                ChangeCounts = (ChangeCounts != null) ? ChangeCountsToString(bot, ChangeCounts) : "",
                AuthorTime = formattedTime,
                Author = transform(Author),
                AuthorName = transform(AuthorName),
                AuthorEmail = transform(AuthorEmail),
                Comment = transform(Comment.Truncate(Settings.CommentMaxLength))
            }));

            return sb.ToString();
        }

        private string ChangeCountsToString(BotElement bot, Dictionary<TfsGitChangeType, int> changeCounts)
        {
            var counters = new[] {
                new ChangeCounter(TfsGitChangeType.Add, bot.Text.ChangeCountAddFormat, 0),
                new ChangeCounter(TfsGitChangeType.Edit, bot.Text.ChangeCountEditFormat, 0),
                new ChangeCounter(TfsGitChangeType.Delete, bot.Text.ChangeCountDeleteFormat, 0),
                new ChangeCounter(TfsGitChangeType.Rename, bot.Text.ChangeCountRenameFormat, 0),
                new ChangeCounter(TfsGitChangeType.SourceRename, bot.Text.ChangeCountSourceRenameFormat, 0)
            };

            foreach (var changeCount in changeCounts)
            {
                // renamed files will also show up as Rename or Rename+Edit, so don't count them twice
                if (changeCount.Key == (TfsGitChangeType.Delete | TfsGitChangeType.SourceRename)) continue;

                foreach (var counter in counters.Where(c => changeCount.Key.HasFlag(c.Type)))
                {
                    counter.Count += changeCount.Value;
                }
            }

            return string.Join(", ", counters.Where(c => c.Count > 0).Select(c => c.Format.FormatWith(new {c.Count })));
        }
    }

    public enum CommitRowType
    {
        Commit,
        RefUpdate
    }

    class ChangeCounter
    {
        public TfsGitChangeType Type { get; set; }
        public string Format { get; set; }
        public int Count { get; set; }

        public ChangeCounter(TfsGitChangeType type, string format, int count)
        {
            Type = type;
            Format = format;
            Count = count;
        }
    }
}

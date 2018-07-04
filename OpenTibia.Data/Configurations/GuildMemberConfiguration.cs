// <copyright file="GuildMemberConfiguration.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenTibia.Data.Models;

    public class GuildMemberConfiguration : IEntityTypeConfiguration<GuildMember>
    {
        public void Configure(EntityTypeBuilder<GuildMember> builder)
        {
            builder.HasKey(b => b.EntryId);

            builder.Property(t => t.AccountId)
                .IsRequired();

            builder.Property(t => t.GuildId)
                .IsRequired();
        }
    }
}

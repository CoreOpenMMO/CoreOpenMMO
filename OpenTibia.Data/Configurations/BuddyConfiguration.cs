// <copyright file="BuddyConfiguration.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenTibia.Data.Models;

    public class BuddyConfiguration : IEntityTypeConfiguration<Buddy>
    {
        public void Configure(EntityTypeBuilder<Buddy> builder)
        {
            builder.HasKey(b => b.EntryId);

            builder.Property(t => t.AccountNr)
                .IsRequired();

            builder.Property(t => t.BuddyId)
                .IsRequired();

            builder.Property(t => t.BuddyString)
                .IsRequired();

            builder.Property(t => t.Timestamp)
                .IsRequired();

            builder.Property(t => t.InitiatingId)
                .IsRequired();
        }
    }
}

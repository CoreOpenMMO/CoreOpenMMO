// <copyright file="DeathConfiguration.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenTibia.Data.Models;

    public class DeathConfiguration : IEntityTypeConfiguration<Death>
    {
        public void Configure(EntityTypeBuilder<Death> builder)
        {
            builder.HasKey(b => b.RecordId);

            builder.Property(t => t.PlayerId)
                .IsRequired();

            builder.Property(t => t.Level)
                .IsRequired();

            builder.Property(t => t.ByPeekay)
                .IsRequired();

            builder.Property(t => t.CreatureString)
                .IsRequired();

            builder.Property(t => t.Timestamp)
                .IsRequired();
        }
    }
}

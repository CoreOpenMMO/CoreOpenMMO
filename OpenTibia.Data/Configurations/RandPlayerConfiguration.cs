// <copyright file="RandPlayerConfiguration.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenTibia.Data.Models;

    public class RandPlayerConfiguration : IEntityTypeConfiguration<RandPlayer>
    {
        public void Configure(EntityTypeBuilder<RandPlayer> builder)
        {
            builder.HasKey(b => b.RandId);

            builder.Property(t => t.AccountId)
                .IsRequired();
        }
    }
}

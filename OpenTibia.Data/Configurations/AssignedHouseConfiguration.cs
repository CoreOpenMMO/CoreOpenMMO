// <copyright file="AssignedHouseConfiguration.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenTibia.Data.Models;

    public class AssignedHouseConfiguration : IEntityTypeConfiguration<AssignedHouse>
    {
        public void Configure(EntityTypeBuilder<AssignedHouse> builder)
        {
            builder.HasKey(b => b.HouseId);

            builder.Property(t => t.PlayerId)
                .IsRequired();

            builder.Property(t => t.OwnerString)
                .IsRequired();

            builder.Property(t => t.Virgin)
                .IsRequired();

            builder.Property(t => t.Gold)
                .IsRequired();

            builder.Property(t => t.World)
                .IsRequired();
        }
    }
}

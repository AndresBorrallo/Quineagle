﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace libQuinEagle
{
	public enum LeagueEnum
	{
		[Description( "Primera" )]
		PRIMERA = 436,

		[Description( "Segunda" )]
		SEGUNDA = 437
	}

	public enum QuinielaResult
	{
		[Description( "Vacio" )]
		VOID = 0,

		[Description( "1" )]
		ONE = 1,

		[Description( "X" )]
		X = 2,

		[Description( "2" )]
		TWO = 4,

		[Description( "1-X" )]
		ONEX = 3,

		[Description( "2-X" )]
		TWOX = 6,

		[Description( "1-X-2" )]
		ONEXTWO = 7
	}

	public static class EnumUtility
	{
		public static string GetDescriptionFromEnumValue( Enum value )
		{
			DescriptionAttribute attribute = value.GetType()
				.GetField( value.ToString() )
				.GetCustomAttributes( typeof( DescriptionAttribute ), false )
				.SingleOrDefault() as DescriptionAttribute;
			return attribute == null ? value.ToString() : attribute.Description;
		}

		public static T GetEnumValueFromDescription<T>( string description )
		{
			var type = typeof( T );
			if( !type.IsEnum )
				throw new ArgumentException();
			FieldInfo[] fields = type.GetFields();
			var field = fields
							.SelectMany( f => f.GetCustomAttributes(
								 typeof( DescriptionAttribute ), false ), (
										f, a ) => new { Field = f, Att = a } )
							.Where( a => ( ( DescriptionAttribute )a.Att )
								 .Description == description ).SingleOrDefault();
			return field == null ? default( T ) : ( T )field.Field.GetRawConstantValue();
		}
	}
}

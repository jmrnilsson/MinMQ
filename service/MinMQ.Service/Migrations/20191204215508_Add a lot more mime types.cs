using Microsoft.EntityFrameworkCore.Migrations;

namespace MinMq.Service.Migrations
{
	public partial class Addalotmoremimetypes : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"insert into ""tMimeTypes"" (""Expression"", ""Added"", ""Changed"") values
				('application/x-7z-compressed', current_timestamp, current_timestamp),
				('application/zip', current_timestamp, current_timestamp),
				('font/woff2', current_timestamp, current_timestamp),
				('font/woff', current_timestamp, current_timestamp),
				('audio/wav', current_timestamp, current_timestamp),
				('text/plain', current_timestamp, current_timestamp),
				('font/ttf', current_timestamp, current_timestamp),
				('image/tiff', current_timestamp, current_timestamp),
				('application/x-tar', current_timestamp, current_timestamp),
				('image/svg+xml', current_timestamp, current_timestamp),
				('application/x-sh', current_timestamp, current_timestamp),
				('application/rtf', current_timestamp, current_timestamp),
				('application/x-rar-compressed', current_timestamp, current_timestamp),
				('application/php', current_timestamp, current_timestamp),
				('application/pdf', current_timestamp, current_timestamp),
				('image/png', current_timestamp, current_timestamp),
				('font/otf', current_timestamp, current_timestamp),
				('application/ogg', current_timestamp, current_timestamp),
				('video/ogg', current_timestamp, current_timestamp),
				('audio/ogg', current_timestamp, current_timestamp),
				('application/vnd.oasis.opendocument.text', current_timestamp, current_timestamp),
				('application/vnd.oasis.opendocument.spreadsheet', current_timestamp, current_timestamp),
				('application/vnd.oasis.opendocument.presentation', current_timestamp, current_timestamp),
				('application/vnd.apple.installer+xml', current_timestamp, current_timestamp),
				('video/mpeg', current_timestamp, current_timestamp),
				('audio/mpeg', current_timestamp, current_timestamp),
				('text/javascript', current_timestamp, current_timestamp),
				('audio/x-midi', current_timestamp, current_timestamp),
				('audio/midi', current_timestamp, current_timestamp),
				('application/ld+json', current_timestamp, current_timestamp),
				('text/javascript', current_timestamp, current_timestamp),
				('image/jpeg', current_timestamp, current_timestamp),
				('application/java-archive', current_timestamp, current_timestamp),
				('text/calendar', current_timestamp, current_timestamp),
				('image/vnd.microsoft.icon', current_timestamp, current_timestamp),
				('text/html', current_timestamp, current_timestamp),
				('image/gif', current_timestamp, current_timestamp),
				('application/gzip', current_timestamp, current_timestamp),
				('application/epub+zip', current_timestamp, current_timestamp),
				('application/vnd.ms-fontobject', current_timestamp, current_timestamp),
				('application/vnd.openxmlformats-officedocument.wordprocessingml.document', current_timestamp, current_timestamp),
				('application/msword', current_timestamp, current_timestamp),
				('text/csv', current_timestamp, current_timestamp),
				('text/css', current_timestamp, current_timestamp),
				('application/x-csh', current_timestamp, current_timestamp),
				('application/x-bzip2', current_timestamp, current_timestamp),
				('application/x-bzip', current_timestamp, current_timestamp),
				('image/bmp', current_timestamp, current_timestamp),
				('application/octet-stream', current_timestamp, current_timestamp),
				('video/x-msvideo', current_timestamp, current_timestamp),
				('audio/aac', current_timestamp, current_timestamp)");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
		}
	}
}

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cinemania
{
    public class Ticket
    {
        public int ID { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public Movie movie { get; set; }

        public Ticket(int id, int row, int column, Movie movie)
        { 
            this.ID = id;
            this.Row = row;
            this.Column = column;
            this.movie = movie;
        }

        public static Ticket GetByID(int ticketID)
        {
            string query = "SELECT * FROM tickets WHERE id = @ticketID";

            MySqlCommand command = new MySqlCommand(query, Store.connection);
            command.Parameters.AddWithValue("@ticketID", ticketID);

            int id = 0;
            int row = 0;
            int column = 0;
            int movie_id = 0;

            Store.connection.Open();
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Store.connection.Close();
                    return null;
                }

                while (reader.Read())
                {
                    id = reader.GetInt32("id");
                    row = reader.GetInt32("row");
                    column = reader.GetInt32("column");
                    movie_id = reader.GetInt32("movie_id");
                }
            }
            Store.connection.Close();

            Movie movie = Movie.GetByID(movie_id);
            Ticket ticket = new Ticket(id, row, column, movie);

            return ticket;
        }

        public static void CancelTicket(int ticketID)
        {
            Ticket ticket = GetByID(ticketID);

            if (ticket == null) 
            {
                MessageBox.Show("Ticket doesn't exist", "Ticket Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ticket.movie.Date < DateTime.Now)
            {
                MessageBox.Show("Ticket has expired", "Ticket Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string createTicketQuery = "DELETE FROM tickets WHERE id = @ticketID";
            MySqlCommand createTicketCommand = new MySqlCommand(createTicketQuery, Store.connection);
            createTicketCommand.Parameters.AddWithValue("@ticketID", ticketID);

            Store.connection.Open();
            createTicketCommand.ExecuteNonQuery();
            Store.connection.Close();

            MessageBox.Show("Ticket returned", "Ticket", MessageBoxButton.OK, MessageBoxImage.Information);

        }

    }
}

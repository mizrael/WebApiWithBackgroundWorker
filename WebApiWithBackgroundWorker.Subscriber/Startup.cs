using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using WebApiWithBackgroundWorker.Common.Messaging;
using WebApiWithBackgroundWorker.Subscriber.Messaging;

namespace WebApiWithBackgroundWorker
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IMessagesRepository, InMemoryMessagesRepository>();
            services.AddSingleton<IConnectionFactory>(ctx =>
            {
                var connStr = this.Configuration["rabbit"];
                return new ConnectionFactory()
                {
                    Uri = new Uri(connStr),
                    DispatchConsumersAsync = true // this is mandatory to have Async Subscribers
                };
            });
            services.AddSingleton<IBusConnection, RabbitPersistentConnection>();
            services.AddSingleton<ISubscriber, RabbitSubscriber>();

            var channel = System.Threading.Channels.Channel.CreateBounded<Message>(100);
            services.AddSingleton(channel);

            services.AddSingleton<IProducer>(ctx =>{
                var channel = ctx.GetRequiredService<System.Threading.Channels.Channel<Message>>();
                var logger = ctx.GetRequiredService<ILogger<Producer>>();
                return new Producer(channel.Writer, logger);
            });

            services.AddSingleton<IEnumerable<IConsumer>>(ctx =>{
                var channel = ctx.GetRequiredService<System.Threading.Channels.Channel<Message>>();
                var logger = ctx.GetRequiredService<ILogger<Consumer>>();
                var repo = ctx.GetRequiredService<IMessagesRepository>();

                var consumers = Enumerable.Range(1, 10)
                                          .Select(i => new Consumer(channel.Reader, logger, i, repo))
                                          .ToArray();
                return consumers;
            });

            services.AddHostedService<BackgroundSubscriberWorker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

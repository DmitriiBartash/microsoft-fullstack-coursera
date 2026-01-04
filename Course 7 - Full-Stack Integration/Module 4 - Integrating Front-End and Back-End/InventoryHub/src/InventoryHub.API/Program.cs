using InventoryHub.API.Endpoints;
using InventoryHub.API.Middleware;
using InventoryHub.Application;
using InventoryHub.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "InventoryHub API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5001",
                "http://localhost:5029",
                "https://localhost:7004",
                "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "InventoryHub API v1"));
}

app.UseCors("AllowClient");

app.MapGet("/", () => Results.Content($$"""
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>InventoryHub API</title>
        <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet">
        <style>
            * { margin: 0; padding: 0; box-sizing: border-box; }
            body {
                font-family: 'Inter', system-ui, sans-serif;
                background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
                min-height: 100vh;
                display: flex;
                align-items: center;
                justify-content: center;
                padding: 20px;
            }
            .container {
                background: rgba(255, 255, 255, 0.03);
                backdrop-filter: blur(10px);
                border: 1px solid rgba(255, 255, 255, 0.1);
                border-radius: 24px;
                padding: 48px;
                max-width: 480px;
                width: 100%;
                box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5);
            }
            .header {
                display: flex;
                align-items: center;
                gap: 16px;
                margin-bottom: 32px;
            }
            .logo {
                width: 56px;
                height: 56px;
                background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
                border-radius: 16px;
                display: flex;
                align-items: center;
                justify-content: center;
                font-size: 24px;
            }
            .title-group h1 {
                color: #f8fafc;
                font-size: 24px;
                font-weight: 700;
            }
            .title-group span {
                color: #64748b;
                font-size: 14px;
            }
            .status-badge {
                display: inline-flex;
                align-items: center;
                gap: 8px;
                background: rgba(34, 197, 94, 0.1);
                border: 1px solid rgba(34, 197, 94, 0.3);
                color: #22c55e;
                padding: 8px 16px;
                border-radius: 100px;
                font-size: 14px;
                font-weight: 500;
                margin-bottom: 32px;
            }
            .status-dot {
                width: 8px;
                height: 8px;
                background: #22c55e;
                border-radius: 50%;
                animation: pulse 2s infinite;
            }
            @keyframes pulse {
                0%, 100% { opacity: 1; }
                50% { opacity: 0.5; }
            }
            .info-grid {
                display: grid;
                gap: 16px;
                margin-bottom: 32px;
            }
            .info-card {
                background: rgba(255, 255, 255, 0.03);
                border: 1px solid rgba(255, 255, 255, 0.06);
                border-radius: 12px;
                padding: 16px;
                display: flex;
                justify-content: space-between;
                align-items: center;
            }
            .info-label {
                color: #64748b;
                font-size: 14px;
            }
            .info-value {
                color: #f1f5f9;
                font-weight: 500;
                font-family: 'SF Mono', 'Fira Code', monospace;
                font-size: 14px;
            }
            .links {
                display: flex;
                gap: 12px;
            }
            .link-btn {
                flex: 1;
                display: flex;
                align-items: center;
                justify-content: center;
                gap: 8px;
                padding: 14px 20px;
                border-radius: 12px;
                text-decoration: none;
                font-weight: 500;
                font-size: 14px;
                transition: all 0.2s ease;
            }
            .link-btn.primary {
                background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
                color: white;
            }
            .link-btn.primary:hover {
                transform: translateY(-2px);
                box-shadow: 0 10px 20px -5px rgba(99, 102, 241, 0.4);
            }
            .link-btn.secondary {
                background: rgba(255, 255, 255, 0.05);
                border: 1px solid rgba(255, 255, 255, 0.1);
                color: #e2e8f0;
            }
            .link-btn.secondary:hover {
                background: rgba(255, 255, 255, 0.1);
            }
            .footer {
                margin-top: 32px;
                padding-top: 24px;
                border-top: 1px solid rgba(255, 255, 255, 0.06);
                text-align: center;
            }
            .footer a {
                color: #6366f1;
                text-decoration: none;
                font-size: 14px;
            }
            .footer a:hover { text-decoration: underline; }
            .footer p { color: #475569; font-size: 12px; margin-top: 8px; }
        </style>
    </head>
    <body>
        <div class="container">
            <div class="header">
                <div class="logo">📦</div>
                <div class="title-group">
                    <h1>InventoryHub API</h1>
                    <span>v1.0.0</span>
                </div>
            </div>

            <div class="status-badge">
                <span class="status-dot"></span>
                All systems operational
            </div>

            <div class="info-grid">
                <div class="info-card">
                    <span class="info-label">Environment</span>
                    <span class="info-value">{{app.Environment.EnvironmentName}}</span>
                </div>
                <div class="info-card">
                    <span class="info-label">Server Time</span>
                    <span class="info-value" id="time">{{DateTime.UtcNow:HH:mm:ss}} UTC</span>
                </div>
                <div class="info-card">
                    <span class="info-label">API Endpoint</span>
                    <span class="info-value">localhost:5000</span>
                </div>
            </div>

            <div class="links">
                <a href="/swagger" class="link-btn primary">Swagger UI</a>
                <a href="/health" class="link-btn secondary">Health</a>
            </div>

            <div class="footer">
                <a href="http://localhost:5001">Open Client App</a>
                <p>InventoryHub Pro &copy; 2025</p>
            </div>
        </div>

        <script>
            setInterval(() => {
                const now = new Date();
                document.getElementById('time').textContent =
                    now.toISOString().slice(11, 19) + ' UTC';
            }, 1000);
        </script>
    </body>
    </html>
    """, "text/html"));

app.MapGet("/health", () => new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName
});

app.MapProductEndpoints();
app.MapCategoryEndpoints();
app.MapSupplierEndpoints();
app.MapClientEndpoints();
app.MapInventoryEndpoints();
app.MapOrderEndpoints();
app.MapSalesOrderEndpoints();
app.MapDashboardEndpoints();

app.Run();

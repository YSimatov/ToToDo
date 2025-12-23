# Подробный разбор кода для защиты (Лабораторные 2-3)

Этот документ создан, чтобы ты понимал каждую строчку своего кода. Читай его как учебник перед защитой.

---

# ЧАСТЬ 1: BACKEND (.NET)

Бэкенд — это "мозги" твоего приложения. Он принимает запросы, обрабатывает данные и отправляет ответы.

## 1. Файл `Program.cs` (Точка входа)
Это самый первый файл, который запускается. Представь, что это **"подготовка кухни перед открытием ресторана"**.

```csharp
var builder = WebApplication.CreateBuilder(args);
```
*   Создаем "строителя" приложения.

```csharp
builder.Services.AddControllers();
```
*   **Что делает:** Говорит программе: "У нас будут Контроллеры (Controllers)". Контроллеры — это классы, которые слушают интернет-запросы (GET, POST).

```csharp
builder.Services.AddSingleton<ITaskRepository, MockTaskRepository>();
```
*   **САМАЯ ВАЖНАЯ СТРОКА ДЛЯ ЛАБ 3!**
*   **Dependency Injection (Внедрение зависимостей):** Мы говорим: "Если кто-то (например, Контроллер) попросит `ITaskRepository` (интерфейс), дай ему `MockTaskRepository` (реализацию)".
*   **Singleton:** Означает, что `MockTaskRepository` создается **один раз** при запуске и живет всё время. Поэтому, когда ты добавляешь задачу, она сохраняется в памяти, пока сервер не выключишь.

```csharp
builder.Services.AddCors(...)
```
*   **CORS:** Разрешение на общение. По умолчанию браузер запрещает сайту на порту 4200 (Angular) общаться с сервером на порту 5288 (.NET). Здесь мы это разрешаем.

---

## 2. Файл `Models/TodoTask.cs` (Модель данных)
Это **"бланк заказа"**. Просто описание того, как выглядит одна Задача.

*   `public string Id { get; set; }` — Уникальный номер.
*   `public string Title { get; set; }` — Название.
*   `public bool Completed { get; set; }` — Галочка "выполнено".
*   Остальные поля (Date, StartTime) — просто данные.

---

## 3. Файл `Repositories/ITaskRepository.cs` (Интерфейс)
Это **"Меню"** или **"Контракт"**.
Здесь нет кода, только список методов, которые *должен* иметь любой репозиторий.

*   `IEnumerable<TodoTask> GetAll();` — Получить все.
*   `void Add(TodoTask task);` — Добавить.
*   И т.д.
*   **Зачем это нужно?** Чтобы в Лаб 4 мы могли легко заменить `MockTaskRepository` на `SqlTaskRepository`, и Контроллер даже не заметит разницы.

---

## 4. Файл `Repositories/MockTaskRepository.cs` (Реализация Лаб 3)
Это **"Фейковая база данных"**.

```csharp
private readonly List<TodoTask> _tasks;
```
*   Мы создаем обычный список `List` в оперативной памяти. Это и есть наше хранилище.

```csharp
public MockTaskRepository() { ... }
```
*   **Конструктор:** При запуске программы мы сразу кладем в список 2 тестовые задачи ("Сделать дизайн" и "Созвон"), чтобы список не был пустым.

```csharp
public void Add(TodoTask task)
{
    if (string.IsNullOrEmpty(task.Id)) task.Id = Guid.NewGuid().ToString();
    _tasks.Add(task);
}
```
*   Если у задачи нет ID, мы придумываем его (Guid — это длинный случайный набор букв и цифр). И добавляем задачу в список.

```csharp
public void Update(TodoTask task)
```
*   Ищем задачу с таким же ID в списке. Если нашли — перезаписываем её поля новыми значениями.

---

## 5. Файл `Controllers/TasksController.cs` (Официант)
Это **"Пульт управления"**. Именно сюда прилетают запросы из браузера.

```csharp
[Route("api/[controller]")]
```
*   Говорит, что этот контроллер доступен по адресу `http://localhost:5288/api/Tasks`.

```csharp
public TasksController(ITaskRepository repository)
```
*   **Конструктор:** Контроллер "просит" дать ему репозиторий. Благодаря `Program.cs`, он получает наш `MockTaskRepository`.

```csharp
[HttpGet]
public ActionResult<IEnumerable<TodoTask>> GetAll()
{
    return Ok(_repository.GetAll());
}
```
*   Когда приходит **GET** запрос -> Вызываем метод `GetAll` у репозитория -> Возвращаем результат со статусом 200 OK.

```csharp
[HttpPost]
public ActionResult<TodoTask> Create(TodoTask task)
```
*   Когда приходит **POST** запрос (с данными задачи) -> Вызываем `Add`.

---

# ЧАСТЬ 2: FRONTEND (ANGULAR)

Фронтенд — это "лицо" приложения.

## 1. Файл `src/app/services/task.ts` (Сервис данных)
Это **самый важный файл** на фронте. Он отвечает за общение с бэкендом.

```typescript
private http = inject(HttpClient);
private apiUrl = 'http://localhost:5288/api/tasks';
```
*   Подключаем инструмент для HTTP-запросов и указываем адрес нашего бэкенда.

```typescript
private tasksSignal = signal<Task[]>([]);
```
*   **Signals (Сигналы):** Это современная фишка Angular. Это "умная переменная".
*   Когда мы меняем данные внутри `tasksSignal`, Angular **автоматически** перерисовывает все места на экране, где эти данные используются. Не нужно вручную обновлять экран.

```typescript
loadTasks() {
    this.http.get<Task[]>(this.apiUrl).subscribe(...)
}
```
*   Делаем GET запрос.
*   `.subscribe(...)`: Мы "подписываемся" на ответ. Как только сервер ответит, сработает код внутри `next`, и мы обновим наш `tasksSignal`.

```typescript
addTask(task) { ... }
```
*   Отправляем POST запрос. Когда сервер скажет "ОК", мы добавляем новую задачу в наш локальный список `tasksSignal`, чтобы она сразу появилась на экране.

---

## 2. Файл `src/app/tasks/task-list/task-list.ts` (Компонент списка)
Это **визуальная часть**.

```typescript
taskService = inject(TaskService);
```
*   Подключаем наш сервис.

```typescript
tasks = this.taskService.getTasksByDate(...);
```
*   Мы просим сервис дать нам задачи только на выбранную дату.

---

## 3. Файл `src/app/tasks/task-list/task-list.html` (Шаблон)
Это **HTML-верстка**.

```html
<div *ngFor="let task of tasks()">
```
*   **Цикл:** "Для каждой задачи из списка `tasks` создай такой блок `div`".

```html
<span [class.completed]="task.completed">
    {{ task.title }}
</span>
```
*   **Интерполяция:** `{{ }}` вставляет текст задачи.
*   **Привязка класса:** Если `task.completed == true`, то элементу добавится CSS-класс `completed` (и он станет зачеркнутым).

---

## 4. Файл `src/app/auth/auth.ts` (Компонент входа)
Пока это **заглушка** для Лаб 6.

*   `isLoginMode`: Переменная, которая переключает вид между "Вход" и "Регистрация".
*   `onSubmit()`: Сейчас он просто сохраняет фейковый токен и перекидывает пользователя на главную страницу (`/app`). В будущем тут будет реальный запрос на сервер.

---

# Главный вопрос преподавателя: "Как данные попадают с сервера на экран?"

**Твой ответ:**
1. Компонент (например, `TaskList`) при загрузке обращается к сервису `TaskService`.
2. `TaskService` делает HTTP GET запрос на Бэкенд (`TasksController`).
3. Бэкенд берет данные из `MockTaskRepository` (списка в памяти) и возвращает их в формате JSON.
4. `TaskService` получает JSON, обновляет `signal` (умную переменную).
5. Angular видит, что сигнал изменился, и автоматически обновляет HTML через цикл `*ngFor`.

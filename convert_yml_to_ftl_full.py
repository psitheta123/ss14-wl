# convert_yml_to_ftl.py
import yaml
import os
import sys
import argparse
import select  # Для Linux/Mac
from pathlib import Path

def wait_for_key_press():
    """
    Ожидает нажатия любой клавиши перед закрытием терминала
    """
    print("\n\nНажмите любую клавишу для выхода...")

    if os.name == 'nt':  # Windows
        msvcrt.getch()
    else:  # Linux/Mac
        if select.select([sys.stdin], [], [], 0)[0]:
            sys.stdin.read(1)
        else:
            # Если stdin не готов, используем input
            input()

def find_all_prototype_files(prototypes_root="Resources/Prototypes"):
    """
    Находит все YAML файлы прототипов в директории Resources/Prototypes/
    """
    prototype_files = []

    if not os.path.exists(prototypes_root):
        print(f"⚠️  Директория {prototypes_root} не найдена")
        return prototype_files

    for root, dirs, files in os.walk(prototypes_root):
        for file in files:
            if file.endswith('.yml') or file.endswith('.yaml'):
                full_path = os.path.join(root, file)
                prototype_files.append(full_path)

    print(f"📁 Найдено {len(prototype_files)} YAML файлов в {prototypes_root}")
    return prototype_files

def build_global_prototype_map(prototypes_root="Resources/Prototypes"):
    """
    Строит глобальную карту всех прототипов из всех файлов в Resources/Prototypes/
    """
    prototype_files = find_all_prototype_files(prototypes_root)
    global_prototype_map = {}

    for file_path in prototype_files:
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                data = yaml.safe_load(f)

            if not data:
                continue

            for item in data:
                if item and 'id' in item and item.get('type', '').lower() == 'entity':
                    global_prototype_map[item['id']] = item

        except Exception as e:
            print(f"⚠️  Ошибка при чтении файла {file_path}: {e}")

    print(f"🗂️  Загружено {len(global_prototype_map)} entity прототипов из всех файлов")
    return global_prototype_map

def get_field_recursive(prototype_map, prototype_id, field_name):
    """
    Рекурсивно ищет поле в прототипе и его родителях
    """
    if prototype_id not in prototype_map:
        return None

    current_prototype = prototype_map[prototype_id]

    # Проверяем текущий прототип
    if field_name in current_prototype and current_prototype[field_name]:
        return current_prototype[field_name]

    # Если поля нет, проверяем родителя
    parent_id = current_prototype.get('parent')
    if parent_id and parent_id in prototype_map:
        return get_field_recursive(prototype_map, parent_id, field_name)

    return None

def format_field_value(value):
    """
    Форматирует значение поля для FTL файла
    Если значение - строка, возвращает как есть (без кавычек)
    """
    if isinstance(value, str):
        return value
    elif isinstance(value, (int, float)):
        return str(value)
    else:
        return str(value)

def get_input_files():
    """
    Запрашивает у пользователя путь к файлу или папке для обработки
    """
    print("\n" + "="*60)
    print("📁 ВЫБОР ФАЙЛОВ ДЛЯ ЛОКАЛИЗАЦИИ")
    print("="*60)

    while True:
        print("\nВыберите вариант:")
        print("1. Обработать один конкретный YAML файл")
        print("2. Обработать все YAML файлы в папке")
        print("3. Обработать все YAML файлы в Resources/Prototypes/")

        choice = input("\nВведите номер варианта (1-3): ").strip()

        if choice == "1":
            return get_single_file()
        elif choice == "2":
            return get_folder_files()
        elif choice == "3":
            return get_all_prototype_files()
        else:
            print("❌ Неверный выбор. Попробуйте снова.")

def get_single_file():
    """
    Запрашивает путь к одному конкретному файлу
    """
    while True:
        file_path = input("\nВведите путь к YAML файлу: ").strip()

        if not file_path:
            print("❌ Путь не может быть пустым!")
            continue

        if not os.path.exists(file_path):
            print("❌ Файл не существует!")
            continue

        if not (file_path.endswith('.yml') or file_path.endswith('.yaml')):
            print("❌ Файл должен быть YAML (.yml или .yaml)!")
            continue

        return [file_path]

def get_folder_files():
    """
    Запрашивает путь к папке и находит все YAML файлы в ней
    """
    while True:
        folder_path = input("\nВведите путь к папке: ").strip()

        if not folder_path:
            print("❌ Путь не может быть пустым!")
            continue

        if not os.path.exists(folder_path):
            print("❌ Папка не существует!")
            continue

        if not os.path.isdir(folder_path):
            print("❌ Это не папка!")
            continue

        # Находим все YAML файлы в папке
        yaml_files = []
        for file in os.listdir(folder_path):
            if file.endswith('.yml') or file.endswith('.yaml'):
                yaml_files.append(os.path.join(folder_path, file))

        if not yaml_files:
            print("❌ В папке не найдено YAML файлов!")
            continue

        print(f"✅ Найдено {len(yaml_files)} YAML файлов:")
        for file in yaml_files:
            print(f"   - {os.path.basename(file)}")

        confirm = input("\nПродолжить с этими файлами? (y/n): ").strip().lower()
        if confirm in ['y', 'yes', 'д', 'да']:
            return yaml_files
        else:
            continue

def get_all_prototype_files():
    """
    Возвращает все файлы из Resources/Prototypes/
    """
    prototype_files = find_all_prototype_files()

    if not prototype_files:
        print("❌ В Resources/Prototypes/ не найдено YAML файлов!")
        return []

    print(f"✅ Найдено {len(prototype_files)} YAML файлов в Resources/Prototypes/")

    confirm = input("\nПродолжить со всеми файлами? (y/n): ").strip().lower()
    if confirm in ['y', 'yes', 'д', 'да']:
        return prototype_files
    else:
        return []

def get_output_location(input_files):
    """
    Запрашивает у пользователя куда сохранять FTL файлы
    """
    print("\n" + "="*60)
    print("📂 ВЫБОР МЕСТА СОХРАНЕНИЯ")
    print("="*60)

    while True:
        print("\nВыберите вариант сохранения:")
        print("1. Сохранить в указанную папку (сохранит структуру папок)")
        print("2. Сохранить все в одну папку")

        choice = input("\nВведите номер варианта (1-2): ").strip()

        if choice == "1":
            return get_structured_output()
        elif choice == "2":
            return get_flat_output()
        else:
            print("❌ Неверный выбор. Попробуйте снова.")

def get_structured_output():
    """
    Запрашивает корневую папку для сохранения с сохранением структуры
    """
    while True:
        output_root = input("\nВведите корневую папку для FTL файлов (например: Resources/Locale/ru): ").strip()

        if not output_root:
            print("❌ Путь не может быть пустым!")
            continue

        # Создаем папку если её нет
        try:
            os.makedirs(output_root, exist_ok=True)
            return output_root, "structured"
        except Exception as e:
            print(f"❌ Ошибка создания папки: {e}")

def get_flat_output():
    """
    Запрашивает одну папку для сохранения всех FTL файлов
    """
    while True:
        output_folder = input("\nВведите папку для FTL файлов: ").strip()

        if not output_folder:
            print("❌ Путь не может быть пустым!")
            continue

        # Создаем папку если её нет
        try:
            os.makedirs(output_folder, exist_ok=True)
            return output_folder, "flat"
        except Exception as e:
            print(f"❌ Ошибка создания папки: {e}")

def convert_single_file(yml_path, ftl_path, global_prototype_map):
    """
    Конвертирует один конкретный YAML файл в FTL с рекурсивным поиском полей
    """
    try:
        # Читаем YAML как сырой текст, чтобы определить уровень вложенности
        with open(yml_path, 'r', encoding='utf-8') as yf:
            raw_lines = yf.readlines()

        # Парсим YAML для получения структуры данных
        with open(yml_path, 'r', encoding='utf-8') as yf:
            data = yaml.safe_load(yf)

        # Создаем директорию для FTL файла если её нет
        ftl_dir = os.path.dirname(ftl_path)
        if ftl_dir and not os.path.exists(ftl_dir):
            os.makedirs(ftl_dir)

        # Анализируем отступы чтобы определить корневые элементы
        root_items = []
        current_item = None
        current_indent = 0

        for line in raw_lines:
            stripped_line = line.strip()
            if not stripped_line or stripped_line.startswith('#'):
                continue

            # Определяем уровень отступа
            indent_level = len(line) - len(line.lstrip())

            if line.lstrip().startswith('- '):
                # Новый элемент списка
                if current_item is not None:
                    root_items.append(current_item)
                current_item = {'line': line, 'indent': indent_level, 'is_root': True}
            elif current_item is not None and indent_level > current_item['indent']:
                # Вложенный элемент - помечаем как не корневой
                current_item['is_root'] = False
            elif current_item is not None and indent_level <= current_item['indent']:
                # Завершаем текущий элемент и начинаем новый
                root_items.append(current_item)
                if line.lstrip().startswith('- '):
                    current_item = {'line': line, 'indent': indent_level, 'is_root': True}
                else:
                    current_item = None

        # Добавляем последний элемент
        if current_item is not None:
            root_items.append(current_item)

        # Записываем FTL
        count = 0
        skipped_count = 0
        with open(ftl_path, 'w', encoding='utf-8') as ff:
            for i, item in enumerate(data):
                if not item or 'id' not in item:
                    continue

                # Проверяем, является ли элемент корневым
                is_root_element = i < len(root_items) and root_items[i]['is_root']

                # Пропускаем если не entity ИЛИ если это вложенный элемент
                if not is_root_element or item.get('type', '').lower() != 'entity':
                    skipped_count += 1
                    continue

                prototype_id = format_field_value(item['id'])

                # Основная строка с ID и именем
                name = get_field_recursive(global_prototype_map, prototype_id, 'name')
                if not name:
                    name = item.get('name', '')

                if name:
                    ff.write(f"ent-{prototype_id} = {format_field_value(name)}\n")
                else:
                    ff.write(f"ent-{prototype_id} = \n")

                # Проверяем локально
                desc = get_field_recursive(global_prototype_map, prototype_id, 'desc')
                if 'desc' in item and item['desc']:
                    ff.write(f"    .desc = {format_field_value(item['desc'])}\n")
                # Если не нашли локально, рекурсивно ищем desc
                elif desc:
                    ff.write(f"    .desc = {format_field_value(desc)}\n")

                # проверяем локально
                suffix = get_field_recursive(global_prototype_map, prototype_id, 'suffix')
                if 'suffix' in item and item['suffix']:
                    ff.write(f"    .suffix = {format_field_value(item['suffix'])}\n")
                # Если не нашли локально, рекурсивно ищем suffix
                elif suffix:
                    ff.write(f"    .suffix = {format_field_value(suffix)}\n")

                # Пустая строка между записями для читаемости
                ff.write("\n")
                count += 1

        print(f"✓ Конвертирован: {os.path.basename(yml_path)} -> {ftl_path}")
        return count, skipped_count

    except Exception as e:
        print(f"✗ Ошибка при конвертации {yml_path}: {e}")
        return 0, 0

def process_files(input_files, output_location, output_type, global_prototype_map):
    """
    Обрабатывает все выбранные файлы
    """
    print("\n" + "="*60)
    print("🔄 НАЧАЛО ЛОКАЛИЗАЦИИ")
    print("="*60)

    total_entities = 0
    total_skipped = 0
    processed_files = 0

    for input_file in input_files:
        # Определяем путь для выходного файла
        if output_type == "structured":
            # Сохраняем структуру папок относительно Resources/Prototypes/
            relative_path = Path(input_file).relative_to("Resources/Prototypes")
            ftl_path = Path(output_location) / relative_path.with_suffix('.ftl')
        else:
            # flat - сохраняем все в одну папку
            filename = Path(input_file).with_suffix('.ftl').name
            ftl_path = Path(output_location) / filename

        # Конвертируем файл
        entities, skipped = convert_single_file(input_file, str(ftl_path), global_prototype_map)
        total_entities += entities
        total_skipped += skipped
        processed_files += 1

    print(f"\n📊 ИТОГИ ЛОКАЛИЗАЦИИ:")
    print(f"   Обработано файлов: {processed_files}")
    print(f"   Добавлено entity: {total_entities}")
    print(f"   Пропущено не-entity: {total_skipped}")
    print(f"   Файлы сохранены в: {output_location}")

def main_interactive():
    """
    Основная функция с интерактивным интерфейсом
    """
    print("🚀 КОНВЕРТЕР YAML В FTL ДЛЯ SPACE STATION 14")
    print("="*60)

    # Шаг 1: Выбор файлов для обработки
    input_files = get_input_files()
    if not input_files:
        print("❌ Не выбрано файлов для обработки!")
        return False

    # Шаг 2: Выбор места сохранения
    output_location, output_type = get_output_location(input_files)

    # Шаг 3: Загрузка глобальной карты прототипов
    print("\n🔄 Загрузка всех прототипов для поиска parent'ов...")
    global_prototype_map = build_global_prototype_map()

    if not global_prototype_map:
        print("❌ Не удалось загрузить прототипы!")
        return False

    # Шаг 4: Обработка файлов
    process_files(input_files, output_location, output_type, global_prototype_map)

    return True

if __name__ == "__main__":
    try:
        # Всегда используем интерактивный режим
        success = main_interactive()

        if success:
            print("\n✅ Локализация завершена успешно!")
        else:
            print("\n❌ Локализация завершена с ошибками!")

    except KeyboardInterrupt:
        print("\n\n⏹️  Операция прервана пользователем")
    except Exception as e:
        print(f"\n💥 Критическая ошибка: {e}")

    finally:
        # Ожидаем нажатия клавиши перед закрытием
        wait_for_key_press()

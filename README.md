# ticomp

Очередная небольшая утилита для склейки содержимого файлов. Может упростить компоновку большого документа позволяя ссылаться на содержимое в других файлах.

## Использование

``` bash
ticomp mainDocument.md > resultDocument.md
```

Результирующее содержание файла выводится в консоль. Поэтому, используется переопределение потока вывода в файл ```> resultDocument.md``` для сохранения результата подстановок.

### Дополнительные опции

Есть возможность задать дополнительные параметры по кодировке документов и имени результирующего файла:

``` bash
ticomp [options] inputfile.md -out="outputfile.md"
```
где **[options]**:

- -ie=code - set input file codepage number;
- -oe=code - set output file codepage number;
- -out=\"output file path\" - set output file path;
- -verb - verbose mode on (please, do not use it in pipeline mode).


## Синтаксис метки

На данный момент описание ссылки на содержимое другого файла делается так:

```
Основной текст документа, основной текст документа, основной текст документа...

{ticomp|include = C:\work\alice\additionalData.txt }

Продолжение документа, продолжение документа, продолжение документа.
```

### Номерные ссылки

Утилита позволяет проставить автоматическую нумерацию ссылок в тексте, например для рисунков, таблиц или формул.

``` markdown
Очень правильное и полезное описание какой-то мега-крутой штуки, но более детальное представление можно посмотреть на рисунке {ticomp|ref-picture = picture-details}. Это подтверждает то что мега-штука все еще крутая.

![Рисунок {ticomp|ref-picture = picture-details} - А это подпись к рисунку, после которого все стало понятно](https://tias.pro/logo-white.svg)
```

На данный момент поддерживается четыре области независимых счетчиков:

- для картинок или иллюстраций _ref-picture_;
- для таблиц _ref-table_;
- для формул _ref-formula_;
- для списков или различных листингов _ref-list_.
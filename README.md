# CVARC

*Фреймворк для проведения виртуальных соревнований между игровыми ИИ*


## Описание

Unity-проект: `uCVARC`

Решение со всеми библиотеками: `Competitions.sln`

Кроме того, для разработчиков HoMM существует `Competitions/Homm/Homm.sln`, 
в котором лежат тесты и утилита для генерации карт.


## Сборка


### Сборка бандлов
*(тем, кто скачал весь репозиторий, этот шаг можно пропустить, так как в репозитории бандлы уже собраны)*

#### Для Pudge:

1. Открыть проект `Competitions/Pudge/PudgeUnityBundleBuilder` с помощью редактора Unity.

2. Выбрать в меню `Assets -> Build AssetBundles`.

#### Для HoMM:

1. Открыть проект `Competitions/Homm/HommUnityBundleBuilder` с помощью редактора Unity.

2. Выбрать в меню `Assets -> Build AssetBundles`.


### Сборка библиотек

1. Открыть `Competitions.sln` в Visual Studio версии не меньше 2015

2. Выбрать в меню Build -> Build Solution


#### Магия

Благодаря тому, что в проектах CVARC.Core, Infrastructure и UnityCommons изменен Output Path, 
получившиеся в результате сборки этих проетов бинарники попадают в каталог uCVARC/Assets/CVARC вместо 
привычного bin/Debug.
После их сборки в юнити появится на них референс и станет возможен запуск Unity-проекта uCVARC.

В проектах HoMM и Pudge также изменен Output Path, и после сборки бинарники попадают в uCVARC/Dlc/Assemblies. 
Из этой директории кварк загружает библиотеки с правилами при помощи рефлексии.

Скопируется много всего, но на самом деле нужны всего два файла: **HoMM.dll** для запуска HoMM и **Pudge.dll** для запуска Pudge.

Также при сборке любого из этих двух проектов соответствующие ему бандлы с ассетами из BundleBuilder'а скопируются 
в uCVARC/Dlc/Bundles, после чего они также станут доступны для загрузки в рантайме.


### Запуск в редакторе Unity

После сборки библиотек станет возможен запуск Unity-проекта. Для этого нужно открыть папку uCVARC с помощью Unity.


### Сборка в Unity

В меню редактора выбрать File -> Build Settings...

Далее выбраем параметры сборки и собираем как обычно.
После сборки копируем каталог Dlc в директорию с собранным экзешником.

```
cvarc
|- cvarc_Data
|- Dlc
|- cvarc.exe
```


## IDlcEntryPoint


Каждая библиотека с правилами соревнований должна содержать в точности один публичный класс, реализующий интерфейс `IDlcEntryPoint`:

```CSharp
namespace UnityCommons
{
    public interface IDlcEntryPoint
    {
        IEnumerable<Competitions> GetLevels();
    }
}
```


## settings.json


При первом запуске кварк создаст в корне проекта конфигурационный файл settings.json со следующим содержимым:

```json
{
    "TutorialCompetitions": "Pudge",
    "TutorialLevel": "Level2",
    "DlcBundles": ["pudge"],
    "DlcAssemblies": ["Pudge.dll"],
    "DebugTypes": ["XXX"]
}
```

`TutorialCompetitions` - имя соревнований, правила которых будут использованы при запуске туториала. 
Должны совпадать с CompetitionsName одного из соревнований, загруженных из библиотек с правилами.

`TutorialLevel` - уровень, используемый при запуске туториала. 
Должен совпадать с одиним из уровней TutorialCompetitions.

`DlcBundles` - имена загружаемых бандлов с ассетами. 
Должны совпадать с тем, что лежит в Dlc/Bundles.

`DlcAssemblies` - имена загружаемых библиотек. 
Должны совпадать с тем, что лежит в Dlc/Assemblies.

`DebugTypes` - имена типов, для которых будут выводиться отладочные сообщения в консоль 
или в *_Data/output_log.txt, когда юнити-проект собран.

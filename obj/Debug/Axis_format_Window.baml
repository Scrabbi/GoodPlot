   M S B A M L   `   `   `  ����  GoodPlot!    GoodPlot.Axis_format_Window   5   	   U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   X TXceed.Wpf.Toolkit, Version=3.3.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5   	     � Title$  Axis_Format_Window��5   	   $��393.133��6$   $��661.833��65   .��5      �  � RowDefinitions 5   
   �� 5      $	��12* �6   6,   �� 5	      $	��25* �6   6,    � ColumnDefinitions 5   
   �� 5      $	��77* �6   6.   �� 5      $
��161* �6   6/   �� 5      $	��73* �6   6.   �� 5      $
��106* �6   6/   �� 5      $
��101* �6   6.   �� 5      $
��136* �6   6.   .+�5   
   �� $��0��6   $��6��6"    �� Orientation$ Vertical=�66   $��Stretch=�6M   .+�5      �� $��Название оси��6   
����� �$��Center=�6=   �� 5      -   5        $��	Axis_name��5      $��TextBox��6'   
�������$��Stretch=�6P   5      �� 5   
   $��1��6   $��0��6"   $��Stretch=�62   .+�5      �� $	��Max��6   
������$��Stretch=�6:   $��35��6X   �� 5      $	��Min��6   
������$��Stretch=�6:   $��34��6X   �� 5      $��Цвет��6   
������$��Stretch=�6;   $��33��6Y   �� 5      $��Размер шрифта��6   
������$��Stretch=�6D   $��27��6b   �� 5      $��Формат чисел��6   
������$��Stretch=�6C   $��27��6a   �� 5      $)��#Количество делений��6   
������$��Stretch=�6I   $��27��6g   �� 5      $��
Офсет��6   
������$��Stretch=�6<   $��27��6Z   �� 5      $��Ширина��6   
������$��Stretch=�6=   $��27��6[   5!      �� 5"   
   $��1��6   $��1��6"   $��Stretch=�62   .+�5#      �� -   5        $��
TextBoxMax��5#      $��35��6(   .��64   5$      �� 5%      -   5        $��
TextBoxMin��5%      $��35��6(   .��64   5&      #  Xceed.Wpf.Toolkit.ColorPicker  5'      -   5        $��ColorBox��5'       $��33��60   �� 5(      -   5        $��FontSizeTextBox��5(      $��28��6-    �� TextWrapping$
 Wrap=�69   $��TextBox��6M   �� 5)      -   5        $��ValueFormatTextBox��5)      $��26��60   $
 Wrap=�6<   $��TextBox��6P   �� 5*      -   5        $��MarksCountTextBox��5*      $��26��6/   $
 Wrap=�6;   $��5��6O   �� 5+      -   5        $��OfsetTextBox��5+      $��26��6*   $
 Wrap=�66   $��5��6J   �� 5,      -	   5        $��WideTextBox��5,      $��26��6)   $
 Wrap=�65   $��5��6I   5-      �� 5/   
   $��1��6   $��2��6"   $��Stretch=�62   .+�50      �� $%��Основные деления��6   
�������$��Stretch=�6I   $��35��6g   �� 51      $��Интервал,c��6   
�������$��Stretch=�6<   $��34��6Z   �� 52      $��Ширина��6   
�������$��Stretch=�68   $��33��6V   �� 53      $��Цвет��6   
�������$��Stretch=�66   $��36��6T   �� 54      $��
Стиль��6   
�������$��Stretch=�67   $��27��6U   �� 55      $��Длина (риски)��6   
�������$��Stretch=�6D   $��27��6b   �� 56      $��Вид (риски)��6   
�������$��Stretch=�6B   $��27��6`   58      �� 59   
   $��1��6   $��3��6"   $��Stretch=�62   .+�5:      �� -
   5        $��TickorGridMarks��5:      $��35��6.   �� 5;      -   5        $��TextBoxInterval��5;      $��32��6-   6e   �� 5<      -   5        $��TextBoxWidth��5<      $��35��6Q   6_     5=      -   5        $��ColorBoxMainTick��5=       $��33��6u   �� 5>      -   5        $��ComboBoxStyleMain��5>      $��28��6f   �� 5?      -   5        $��TextBoxLength��5?      $��30��6+   6a   �� 5@      -   5        $��ComboBoxView��5@      $��27��6W   5C      �� 5G   
   $��1��6   $��4��6"   $��Stretch=�62   .+�5H      �� $/��)Промежуточные деления��6   
�������$��Stretch=�6N   $��35��6l   �� 5I      $��Интервал,c��6   
�������$��Stretch=�6<   $��34��6Z   �� 5J      $��Ширина��6   
�������$��Stretch=�68   $��33��6V   �� 5K      $��Цвет��6   
�������$��Stretch=�66   $��36��6T   �� 5L      $��
Стиль��6   
�������$��Stretch=�67   $��27��6U   �� 5M      $��Длина (риски)��6   
�������$��Stretch=�6D   $��27��6b   �� 5N      $��Вид (риски)��6   
�������$��Stretch=�6B   $��27��6`   5O      �� 5P   
   $��1��6   $��5��6"   $��Stretch=�62   .+�5Q      �� -   5        $��TickorGridMarks_Minor��5Q      $��38��64   �� 5R      -   5        $��TextBoxInterval_Minor��5R      $��33��63   6q   �� 5S      -   5        $��TextBoxWidth_Minor��5S      $��35��6]   6k     5T      -   5        $��ColorBox_Minor��5T       $��33��6q   �� 5U      -   5        $��ComboBoxStyle_Minor��5U      $��28��6n   �� 5V      -   5        $��TextBoxLength_Minor��5V      $��24��61   6m   �� 5W      -   5        $��ComboBoxView_Minor��5W      $��28��6c   5Y      5Z      5[      
   M S B A M L   `   `   `  ����  GoodPlot    GoodPlot.ModuleDiffEff   5   	   U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  ModuleDiffEff��5   	   $	��406��66   $	��300��6C    � Topmost . $��Right=�6^    � WindowStartupLocation$ Manual=�6z    � Left$	 800��6�   .��5      �  � ColumnDefinitions 5   
   �� 5      $	��22* �6   �� 5      $	��51* �6   .+�5
   
   ��  �� Orientation$ Vertical=�6   $��Stretch=�6,   $��2��6H   .+�5      �� $(��"Группа для расчета��6   
�������$��Center=�6E   $��Stretch=�6i   �� 5      -   5        $��GroupTexBox��5      $��Center=�6*   $��Stretch=�6N   $��29��6l   �� 5      $4��.Реактивность для расчета��6   
�������$��Center=�6K   $��Stretch=�6o   �� 5      -   5        $��ReactivitiTexBox��5      $��Center=�60   $��Stretch=�6T   $��29��6r   �� 5      -   5         ��Name$ PanelRadioButt��5      $ 
Horizontal=�61   .+�5      	� -   5        $��RadHand��5   2    	� 	GroupName$ CalcWay��6   $��Вручную��6A    d� 	IsChecked$
 TrueY�6S   $��Center=�6e   $��Stretch=�6�   $	��144��6�   	� 5      -   5        $��RadAuto��5   2   $ CalcWay��6   $��Автомат��6A   $��Center=�6n   $��Stretch=�6�   $	��145��6�   5      �� 5      $��Вехние точки��6   
�������$��Center=�6?   $��Stretch=�6c   �� 5      -   5        $ PanelButtsTop��5      $ 
Horizontal=�60   $��1q�6I   $��34��6T   	 J� RenderTransformOrigin$	 0.503,1.147&�6`   .+�5      �� -   5        
 ��Name$
 ButtonTopStart��5   0   $��Верх:начало��6   $��Center=�6H   $��Stretch=�6l   $	��144��6�   
�������$��34��5   C   $��Bottom=�6O   �� 5      -	   5        $
 ButtonTopEnd��5   0   $��Верх:конец��6   $��Center=�6F   $��Stretch=�6j   $	��145��6�   
�������5   &   5      �� 5      $��Нижние точки��6   
�������$��Center=�6?   $��Stretch=�6c   �� 5      -
   5        $ PanelButtsBottom��5      $ 
Horizontal=�63   $��1q�6L   $��34��6W   $	 0.503,1.147&�6c   .+�5      �� -   5        $
 ButtonBotStart��5   /   $��Низ:начало��6   $��Center=�6G   $��Stretch=�6k   $	��144��6�   
�������5   &   �� 5      -   5        $
 ButtonBotEnd��5   /   $��Низ:конец��6   $��Center=�6E   $��Stretch=�6i   $	��145��6�   
�������5   &   5       �� 5!      -   5        $��ButtonGroupPoint��5!      $C��=Отметить середну движения группы��6-   $��1q�6Y   $��3q�6d   
�������$��Center=�5"      $��Stretch=�69   6W   �� 5#      -   5        $��ButtonCalculation��5#      $��Рассчитать��6.   $
��1,10q�6C   $��3q�6Q   
�������$��Center=�6t   $��Stretch=�5$      62   �� 5%      -   5        $@��:Отобразить сглаживающие прямые��5%   *   $��1q�6S   $��3q�6^   
�������$��Center=�6�   $��Stretch=�6�   �� 5&      $��
Сброс��6   $��1q�6$   $��3q�6/   
�������$��Center=�6R   $��Stretch=�6v   5'      5(      5)      